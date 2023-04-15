using AutoMapper;
using Microsoft.Extensions.Localization;
using NetApp.Application.Dtos.Identity;
using NetApp.Application.Interfaces.Identity;
using NetApp.Domain.Exceptions;
using NetApp.Domain.Models;
using NetApp.Domain.Repositories;
using NetApp.Infrastructure.Contexts;
using NetApp.Shared;

namespace NetApp.Infrastructure.Identity.Services;

internal class RoleService : IRoleService
{
    private readonly NetAppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<NetAppLocalizer> _localizer;

    public RoleService(INetAppDbContext dbContext, IMapper mapper, ICurrentUserService currentUserService, IStringLocalizer<NetAppLocalizer> localizer)
    {
        _dbContext=(NetAppDbContext)dbContext;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _localizer = localizer;
    }
    public async Task<IResponse> DeleteAsync(string id)
    {
        var role = await _dbContext.Roles.FindAsync(id);
        if (role == null) throw new NotFoundException(_localizer["Invalid role id."]);
        _dbContext.Roles.Remove(role);
        await _dbContext.SaveChangesAsync();
        return Response.Success(_localizer["Role deleted successfully."]);
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
