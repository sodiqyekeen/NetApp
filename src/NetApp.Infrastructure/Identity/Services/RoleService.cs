using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using NetApp.Domain.Exceptions;
using NetApp.Infrastructure.Contexts;
using NetApp.Infrastructure.Hubs;
using NetApp.Shared;
using System.Security.Claims;

namespace NetApp.Infrastructure.Identity.Services;

internal class RoleService(NetAppDbContext dbContext,
RoleManager<NetAppRole> roleManager,
ISessionService currentUserService,
IStringLocalizer<NetAppLocalizer> localizer,
ILogger<RoleService> logger,
IHubContext<NetAppHub> hubContext)
 : IRoleService
{
    public async Task<IResponse> DeleteAsync(string id)
    {
        var role = await dbContext.Roles.Include(r => r.UserRoless).FirstOrDefaultAsync(r => r.Id == id) ?? throw new NotFoundException(localizer["Invalid role id."]);
        if (role.Name == DomainConstants.Role.SuperAdmin)
            throw new ApiException(localizer["This role cannot be deleted."]);
        if (role.UserRoless.Any(ur => ur.UserId == currentUserService.UserId))
            throw new ApiException(localizer["You cannot delete a role that you belong."]);
        await roleManager.DeleteAsync(role);
        await dbContext.SaveChangesAsync();
        await hubContext.Clients.All.SendAsync(SharedConstants.SignalR.OnRoleDeleted, role.Name);
        return Response.Success(localizer["Role deleted successfully."]);
    }

    public async Task<IResponse<List<RoleResponse>>> GetAllAsync()
    {
        var roleQuery = roleManager.Roles;
#if !DEBUG
        roleQuery = roleQuery.Where(x => x.Name != DomainConstants.Role.SuperAdmin);
#endif
        var roles = await roleQuery.ProjectToRoleResponse().ToListAsync();
        return Response<List<RoleResponse>>.Success(roles);
    }

    public async Task<IResponse<List<RoleWithPermissionsResponse>>> GetAllRolesWithPermissionsAsync(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return Response<List<RoleWithPermissionsResponse>>.Fail(localizer["Request cancelled."]);

        var allPermissions = PermissionHelper.GetAllPermissions().SelectMany(mp => mp.Permissions).ToDictionary(p => p.Value, p => p.Description);
        var query = roleManager.Roles.Include(r => r.RoleClaims.Where(c => c.ClaimType == SharedConstants.CustomClaimTypes.Permission));
#if !DEBUG
        query = query.Where(x => x.Name != DomainConstants.Role.SuperAdmin);
#endif
        var response = new List<RoleWithPermissionsResponse>();
        var roles = await query.ToListAsync(cancellationToken);
        foreach (var role in roles)
        {
            var roleWithPermissions = new RoleWithPermissionsResponse(role.Id, role.Name!, role.Description, []);
            foreach (var claim in role.RoleClaims)
            {
                if (!allPermissions.TryGetValue(claim.ClaimValue!, out var description)) continue;
                roleWithPermissions.Permissions.Add(description);
            }
            response.Add(roleWithPermissions);
        }
        return Response<List<RoleWithPermissionsResponse>>.Success(response);
    }

    public async Task<IResponse<PermissionResponse>> GetAllPermissionsAsync(string roleId)
    {
        var allPermissions = PermissionHelper.GetAllPermissions().ToList();
        var role = await dbContext.Roles.Include(r => r.RoleClaims.Where(c => c.ClaimType == SharedConstants.CustomClaimTypes.Permission)).FirstOrDefaultAsync(r => r.Id == roleId) ?? throw new NotFoundException(localizer["Invalid role id."]);
        var authorizedClaims = role.RoleClaims.ToDictionary(c => c.ClaimValue!, c => c);

        foreach (var permission in allPermissions.SelectMany(a => a.Permissions))
        {
            if (authorizedClaims.TryGetValue(permission.Value, out var roleClaim))
                permission.Selected = true;
        }

        var response = new PermissionResponse
        {
            RoleId = role.Id,
            RoleName = role.Name!,
            RoleDescription = role.Description,
            RolePermissions = allPermissions
        };
        return Response<PermissionResponse>.Success(response);
    }

    public async Task<IResponse<RoleResponse>> GetByIdAsync(string id)
    {
        var role = await roleManager.FindByIdAsync(id) ?? throw new NotFoundException(localizer["Invalid role id."]);
        return Response<RoleResponse>.Success(role.ToRoleResponse());
    }

    public async Task<IResponse<string>> SaveAsync(RoleRequest request)
    {
        if (request.Id == null)
            return await CreateRoleAsync(request);

        var role = await roleManager.FindByIdAsync(request.Id) ?? throw new NotFoundException(localizer["Invalid role id."]);
        if (role.Name == DomainConstants.Role.SuperAdmin)
            throw new ApiException(localizer["This role cannot be updated."]);
        return await UpdateRoleAsync(request, role);
    }

    private async Task<IResponse<string>> UpdateRoleAsync(RoleRequest request, NetAppRole role)
    {
        role.Name = request.Name!;
        role.Description = request.Description!;
        role.NormalizedName = request.Name!.ToUpper();
        var response = await roleManager.UpdateAsync(role);
        if (response.Succeeded)
            return Response<string>.Success(role.Id, localizer["Role updated successfully."]);

        logger.LogError("Unable to update role. Errors: {0}", response.Errors);
        throw new ApiException(localizer["Unable to update role."]);
    }

    private async Task<IResponse<string>> CreateRoleAsync(RoleRequest request)
    {
        if (await roleManager.RoleExistsAsync(request.Name!))
            throw new ApiException(localizer["Role already exists."]);
        var role = new NetAppRole(request.Name!, request.Description!);
        var response = await roleManager.CreateAsync(role);
        if (response.Succeeded)
            return Response<string>.Success(role.Id, localizer["Role created successfully."]);
        logger.LogError("Unable to create role. Errors: {Errors}", response.Errors);
        throw new ApiException(localizer["Unable to create role."]);
    }

    public async Task<IResponse> UpdatePermissionsAsync(PermissionRequest request)
    {
        var role = await roleManager.FindByIdAsync(request.RoleId!) ?? throw new NotFoundException(localizer["Invalid role id."]);
        if (role.Name == DomainConstants.Role.SuperAdmin)
            throw new ApiException(localizer["Permissions for this role cannot be updated."]);
        if (role.Name == DomainConstants.Role.Admin && currentUserService.Roles.All(r => r != DomainConstants.Role.SuperAdmin))
            throw new ApiException(localizer["Permissions for this role cannot be updated."]);

        var newPermissions = request.Permissions.Where(p => p.Selected).Select(p => p.Value).ToList();
        if (role.Name == DomainConstants.Role.Admin)
            EnsureAdminRoleHasRequiredPermissions(newPermissions);

        var currentRolePermissions = await roleManager.GetClaimsAsync(role);
        //remove old permissions
        foreach (var permission in currentRolePermissions)
        {
            await roleManager.RemoveClaimAsync(role, permission);
        }
        //add new permissions
        foreach (var permission in newPermissions)
        {
            await roleManager.AddClaimAsync(role, new Claim(SharedConstants.CustomClaimTypes.Permission, permission));
        }
        await hubContext.Clients.All.SendAsync(SharedConstants.SignalR.OnPermissionUpdated, role.Name);
        return Response.Success(localizer["Permissions updated successfully."]);
    }

    private void EnsureAdminRoleHasRequiredPermissions(List<string> newPermissions)
    {
        if (newPermissions.Count == 0)
            throw new ApiException(localizer["Admin role must have at least one permission."]);
        //TODO: check if admin role has all user's and role permissions
        if (!newPermissions.ContainsAll(Permissions.User.View, Permissions.Role.View))
            throw new ApiException(localizer["Admin role must have all user's and role permissions."]);
    }


}
