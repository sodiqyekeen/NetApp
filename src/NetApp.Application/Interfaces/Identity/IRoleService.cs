namespace NetApp.Application.Services;

public interface IRoleService
{
    Task<IResponse> DeleteAsync(string id);
    Task<IResponse<List<RoleResponse>>> GetAllAsync();
    Task<IResponse<PermissionResponse>> GetAllPermissionsAsync(string roleId);
    Task<IResponse<RoleResponse>> GetByIdAsync(string id);
    Task<IResponse<string>> SaveAsync(RoleRequest request);
    Task<IResponse> UpdatePermissionsAsync(PermissionRequest request);
    Task<IResponse<List<RoleWithPermissionsResponse>>> GetAllRolesWithPermissionsAsync(CancellationToken cancellationToken);
}
