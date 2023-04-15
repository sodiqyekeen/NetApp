using NetApp.Application.Dtos.Identity;
using NetApp.Application.Interfaces.Identity;
using NetApp.Domain.Models;

namespace NetApp.Infrastructure.Identity.Services;

internal class RoleService : IRoleService
{
    public Task<IResponse> DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<IResponse<List<RoleResponse>>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IResponse<PermissionResponse>> GetAllPermissionsAsync(string roleId)
    {
        throw new NotImplementedException();
    }

    public Task<IResponse<RoleResponse>> GetByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetCountAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IResponse<string>> SaveAsync(RoleRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<IResponse> UpdatePermissionsAsync(PermissionRequest request)
    {
        throw new NotImplementedException();
    }
}
