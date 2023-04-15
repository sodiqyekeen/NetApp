using NetApp.Application.Dtos.Identity;
using NetApp.Domain.Models;

namespace NetApp.Application.Interfaces.Identity;

public interface IRoleClaimService
{
    Task<IResponse<List<RoleClaimResponse>>> GetAllAsync();
    Task<int> GetCountAsync();
    Task<IResponse<RoleClaimResponse>> GetByIdAsync(int id);
    Task<IResponse<List<RoleClaimResponse>>> GetAllByRoleIdAsync(string roleId);
    Task<IResponse> SaveAsync(RoleClaimRequest request);
    Task<IResponse> DeleteAsync(int id);
}
