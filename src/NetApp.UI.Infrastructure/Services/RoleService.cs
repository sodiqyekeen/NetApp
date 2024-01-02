using Microsoft.Extensions.Localization;
using NetApp.Shared;
using NetApp.UI.Infrastructure.Services;

public interface IRoleService
{
    Task<IEnumerable<RoleResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<RoleResponse?> GetByIdAsync(string id);
    Task<IResponse<string>> SaveAsync(RoleRequest request);
    Task<IResponse> DeleteRoleAsync(string id);
    Task<PermissionResponse> GetAllPermissionsAsync(string id);
    Task<IResponse> UpdatePermissionsAsync(PermissionRequest request);
    Task<IEnumerable<RoleWithPermissionsResponse>> GetAllRolesWithPermissionsAsync(CancellationToken cancellationToken);
}

public class RoleService(HttpClient httpClient, ISnackbar snackbar, IStringLocalizer<NetAppLocalizer> localizer) :
BaseService(httpClient, snackbar, localizer), IRoleService
{
    public async Task<IEnumerable<RoleResponse>> GetAllAsync(CancellationToken cancellationToken) =>
    await GetAsync<IEnumerable<RoleResponse>>(Endpoints.Role.GetAll, cancellationToken);

    public async Task<PermissionResponse> GetAllPermissionsAsync(string id) =>
     await GetAsync<PermissionResponse>(Endpoints.Role.GetAllPermissions(id));

    public async Task<IEnumerable<RoleWithPermissionsResponse>> GetAllRolesWithPermissionsAsync(CancellationToken cancellationToken) =>
        await GetAsync<List<RoleWithPermissionsResponse>>(Endpoints.Role.GetAllRolesWithPermissions, cancellationToken);

    public async Task<RoleResponse?> GetByIdAsync(string id) =>
    await GetAsync<RoleResponse>(Endpoints.Role.GetById(id));

    public async Task<IResponse<string>> SaveAsync(RoleRequest request) =>
    await PostAsync<RoleRequest, string>(Endpoints.Role.Save, request);

    public async Task<IResponse> UpdatePermissionsAsync(PermissionRequest request) =>
    await PutAsync(Endpoints.Role.UpdatePermissions(request.RoleId!), request);

    public async Task<IResponse> DeleteRoleAsync(string id) =>
    await DeleteAsync(Endpoints.Role.DeleteRole(id));
}