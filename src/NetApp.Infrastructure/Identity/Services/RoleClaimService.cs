using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using NetApp.Application.Dtos.Identity;
using NetApp.Application.Interfaces.Identity;
using NetApp.Domain.Common;
using NetApp.Domain.Exceptions;
using NetApp.Domain.Models;
using NetApp.Domain.Repositories;
using NetApp.Infrastructure.Contexts;
using NetApp.Infrastructure.Identity.Models;
using NetApp.Shared;

namespace NetApp.Infrastructure.Identity.Services;


internal class RoleClaimService : IRoleClaimService
{
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly NetAppDbContext _db;
    private readonly IStringLocalizer<NetAppLocalizer> _localizer;

    public RoleClaimService(
        IMapper mapper,
        ICurrentUserService currentUserService,
        INetAppDbContext db, IStringLocalizer<NetAppLocalizer> localizer)
    {
        _mapper = mapper;
        _currentUserService = currentUserService;
        _db = (NetAppDbContext)db;
        _localizer = localizer;
    }
    public async Task<IResponse> DeleteAsync(int id)
    {
        var existingRoleClaim = await _db.RoleClaims
              .Include(x => x.Role)
              .FirstOrDefaultAsync(x => x.Id == id);
        if (existingRoleClaim == null)
            throw new ApiException(_localizer["Role Claim does not exist."]);

        _db.RoleClaims.Remove(existingRoleClaim);
        await _db.SaveChangesAsync(_currentUserService.UserId);
        return Response.Success(_localizer[$"Role Claim {existingRoleClaim.ClaimValue} for {existingRoleClaim.Role?.Name} Role deleted."]);
    }

    public async Task<IResponse<List<RoleClaimResponse>>> GetAllAsync()
    {
        var roleClaims = await _db.RoleClaims.ToListAsync();
        var roleClaimsResponse = _mapper.Map<List<RoleClaimResponse>>(roleClaims);
        return Response<List<RoleClaimResponse>>.Success(roleClaimsResponse);
    }

    public async Task<IResponse<List<RoleClaimResponse>>> GetAllByRoleIdAsync(string roleId)
    {
        var roleClaims = await _db.RoleClaims
               .Include(x => x.Role)
               .Where(x => x.RoleId == roleId)
               .ToListAsync();
        var roleClaimsResponse = _mapper.Map<List<RoleClaimResponse>>(roleClaims);
        return Response<List<RoleClaimResponse>>.Success(roleClaimsResponse);
    }
   
    
    public async Task<IResponse<RoleClaimResponse>> GetByIdAsync(int id)
    {
        var roleClaim = await _db.RoleClaims.SingleOrDefaultAsync(x => x.Id == id);
        Guard.AgainstNull(roleClaim, _localizer["Invalid role id."]);
        var roleClaimResponse = _mapper.Map<RoleClaimResponse>(roleClaim);
        return Response<RoleClaimResponse>.Success(roleClaimResponse);
    }

    public async Task<int> GetCountAsync() => await _db.RoleClaims.CountAsync();

    public async Task<IResponse> SaveAsync(RoleClaimRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RoleId))
            throw new ApiException(_localizer["Role is required."]);

        if (request.Id == null)
        {
            await EnsureRoleClaimDoesNotExist(request);

            var roleClaim = _mapper.Map<NetAppRoleClaim>(request);
            await _db.RoleClaims.AddAsync(roleClaim);
            await _db.SaveChangesAsync(_currentUserService.UserId);
            return Response.Success($"Role Claim {request.Value} created.");
        }


        var existingRoleClaim = await _db.RoleClaims.Include(x => x.Role).SingleOrDefaultAsync(x => x.Id == request.Id);
        Guard.AgainstNull(existingRoleClaim, _localizer["Role Claim does not exist."]);

        existingRoleClaim!.ClaimType = request.Type;
        existingRoleClaim.ClaimValue = request.Value;
        existingRoleClaim.Group = request.Group!;
        existingRoleClaim.Description = request.Description!;
        existingRoleClaim.RoleId = request.RoleId;
        _db.RoleClaims.Update(existingRoleClaim);
        await _db.SaveChangesAsync(_currentUserService.UserId);
        return Response.Success(_localizer[$"Role Claim {request.Value} for Role {existingRoleClaim.Role?.Name} updated."]);

    }

    private async Task EnsureRoleClaimDoesNotExist(RoleClaimRequest request)
    {
        var existingRoleClaim = await _db.RoleClaims.
            SingleOrDefaultAsync(x => x.RoleId == request.RoleId && x.ClaimType == request.Type && x.ClaimValue == request.Value);
        if (existingRoleClaim == null) return;
        throw new ApiException("Similar Role Claim already exists.");
    }
}
