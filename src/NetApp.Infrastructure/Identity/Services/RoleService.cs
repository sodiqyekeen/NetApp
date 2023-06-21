using AutoMapper;
using Microsoft.Extensions.Localization;
using NetApp.Domain.Exceptions;
using NetApp.Domain.Models;
using NetApp.Domain.Repositories;
using NetApp.Infrastructure.Contexts;
using NetApp.Shared;
using NetApp.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetApp.Domain.Constants;
using System.Security.Claims;
using NetApp.Application;

namespace NetApp.Infrastructure.Identity.Services;

internal class RoleService : IRoleService
{
    private readonly NetAppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ISessionService _currentUserService;
    private readonly IStringLocalizer<NetAppLocalizer> _localizer;
    private readonly RoleManager<NetAppRole> _roleManager;
    private readonly ILogger<RoleService> _logger;

    public RoleService(INetAppDbContext dbContext, IMapper mapper, RoleManager<NetAppRole> roleManager, ISessionService currentUserService, IStringLocalizer<NetAppLocalizer> localizer, ILogger<RoleService> logger)
    {
        _dbContext = (NetAppDbContext)dbContext;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _localizer = localizer;
        _roleManager = roleManager;
        _logger = logger;
    }
    public async Task<IResponse> DeleteAsync(string id)
    {
        var role = await _dbContext.Roles.FindAsync(id) ?? throw new NotFoundException(_localizer["Invalid role id."]);
        _dbContext.Roles.Remove(role);
        await _dbContext.SaveChangesAsync();
        return Response.Success(_localizer["Role deleted successfully."]);
    }

    public async Task<IResponse<List<RoleResponse>>> GetAllAsync()
    {
        var roles = (await _roleManager.Roles.Where(x => x.Name != DomainConstants.Role.SuperAdmin).ToListAsync());
        // if (!_currentUserService.IsSuperAdmin)
        //     roles = roles.Where(x => x.Name != ApplicationConstants.Role.SuperAdmin).ToList();
        var rolesResponse = _mapper.Map<List<RoleResponse>>(roles);
        return Response<List<RoleResponse>>.Success(rolesResponse);
    }

    public async Task<IResponse<PermissionResponse>> GetAllPermissionsAsync(string roleId)
    {
        var allPermissions = PermissionHelper.GetAllPermissions().ToList();
        var role = await _dbContext.Roles.Include(r => r.RoleClaims).FirstOrDefaultAsync(r => r.Id == roleId) ?? throw new NotFoundException(_localizer["Invalid role id."]);
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
        var role = await _roleManager.FindByIdAsync(id) ?? throw new NotFoundException(_localizer["Invalid role id."]);
        var roleResponse = _mapper.Map<RoleResponse>(role);
        return Response<RoleResponse>.Success(roleResponse);
    }

    public async Task<IResponse<string>> SaveAsync(RoleRequest request)
    {
        if (request.Id == null)
            return await CreateRoleAsync(request);

        var role = await _roleManager.FindByIdAsync(request.Id) ?? throw new NotFoundException(_localizer["Invalid role id."]);
        return await UpdateRoleAsync(request, role);
    }

    private async Task<IResponse<string>> UpdateRoleAsync(RoleRequest request, NetAppRole role)
    {
        role.Name = request.Name!;
        role.Description = request.Description!;
        role.NormalizedName = request.Name!.ToUpper();
        var response = await _roleManager.UpdateAsync(role);
        if (response.Succeeded)
            return Response<string>.Success(role.Id, _localizer["Role updated successfully."]);
        _logger.LogError("Unable to update role. Errors: {0}", response.Errors);
        throw new ApiException(_localizer["Unable to update role."]);
    }

    private async Task<IResponse<string>> CreateRoleAsync(RoleRequest request)
    {
        if (await _roleManager.RoleExistsAsync(request.Name!))
            throw new ApiException(_localizer["Role already exists."]);
        var role = new NetAppRole(request.Name!, request.Description!);
        var response = await _roleManager.CreateAsync(role);
        if (response.Succeeded)
            return Response<string>.Success(role.Id, _localizer["Role created successfully."]);
        _logger.LogError("Unable to create role. Errors: {0}", response.Errors);
        throw new ApiException(_localizer["Unable to create role."]);
    }

    public async Task<IResponse> UpdatePermissionsAsync(PermissionRequest request)
    {
        var role = await _roleManager.FindByIdAsync(request.RoleId!) ?? throw new NotFoundException(_localizer["Invalid role id."]);
        if (role.Name == DomainConstants.Role.SuperAdmin)
            throw new ApiException(_localizer["Permissions for this role cannot be updated."]);
        if (role.Name == DomainConstants.Role.Admin && _currentUserService.Role != DomainConstants.Role.SuperAdmin)
            throw new ApiException(_localizer["Permissions for this role cannot be updated."]);

        var newPermissions = request.Permissions.Where(p => p.Selected).Select(p => p.Value).ToList();
        if (role.Name == DomainConstants.Role.Admin)
            EnsureAdminRoleHasRequiredPermissions(newPermissions);

        var currentRolePermissions = await _roleManager.GetClaimsAsync(role);
        //remove old permissions
        foreach (var permission in currentRolePermissions)
        {
            await _roleManager.RemoveClaimAsync(role, permission);
        }
        //add new permissions
        foreach (var permission in newPermissions)
        {
            await _roleManager.AddClaimAsync(role, new Claim(SharedConstants.CustomClaimTypes.Permission, permission));
        }
        return Response.Success(_localizer["Permissions updated successfully."]);
    }

    private void EnsureAdminRoleHasRequiredPermissions(List<string> newPermissions)
    {
        if (newPermissions.Count == 0)
            throw new ApiException(_localizer["Admin role must have at least one permission."]);
        //TODO: check if admin role has all user's and role permissions
        if (!newPermissions.IsSupersetOf(Permissions.User.View, Permissions.Role.View))
            throw new ApiException(_localizer["Admin role must have all user's and role permissions."]);
    }
}
