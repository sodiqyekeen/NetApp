using Microsoft.AspNetCore.Authorization;
using NetApp.Infrastructure.Identity;
using NetApp.Shared.Constants;

namespace NetApp.Api;
internal static class RoleEndpoints
{
    public static RouteGroupBuilder MapRoleEndpoints(this RouteGroupBuilder group)
    {
        group = group.MapGroup("roles");
        group.MapGet("/", [Authorize(Policy = Permissions.Role.View)] async (IRoleService roleService) =>
        Results.Ok(await roleService.GetAllAsync()))
        .Produces<IResponse<IEnumerable<RoleResponse>>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", async (string id, IRoleService roleService) =>
        Results.Ok(await roleService.GetByIdAsync(id)))
        .Produces<IResponse<RoleResponse>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", [Authorize(Policy = Permissions.Role.Create)] async (RoleRequest request, IRoleService roleService) =>
        Results.Ok(await roleService.SaveAsync(request)))
        .Produces<IResponse<string>>(StatusCodes.Status200OK);

        group.MapPut("/{id}", [Authorize(Policy = Permissions.Role.Edit)] async (string id, RoleRequest request, IRoleService roleService) =>
        Results.Ok(await roleService.SaveAsync(request)))
        .Produces<IResponse<string>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id}", [Authorize(Policy = Permissions.Role.Delete)] async (string id, IRoleService roleService) =>
        Results.Ok(await roleService.DeleteAsync(id)))
        .Produces<IResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{id}/permissions", [Authorize(Policy = Permissions.Permission.View)] async (string id, IRoleService roleService) =>
        Results.Ok(await roleService.GetAllPermissionsAsync(id)))
        .Produces<IResponse<PermissionResponse>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id}/permissions", [Authorize(Policy = Permissions.Role.ManageRolePermission)] async (string id, PermissionRequest request, IRoleService roleService) =>
        Results.Ok(await roleService.UpdatePermissionsAsync(request)))
        .Produces<IResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/permissions", [Authorize] async (IRoleService roleService, CancellationToken cancellationToken) =>
        Results.Ok(await roleService.GetAllRolesWithPermissionsAsync(cancellationToken)))
        .Produces<IResponse<IEnumerable<RoleWithPermissionsResponse>>>(StatusCodes.Status200OK);

        return group;
    }
}
