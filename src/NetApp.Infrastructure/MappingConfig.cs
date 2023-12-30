using Riok.Mapperly.Abstractions;

namespace NetApp.Infrastructure;

[Mapper]
public static partial class MappingConfig
{
    public static partial IQueryable<UserDto> ProjectToUserDto(this IQueryable<NetAppUser> source);
    public static partial UserDto ToUserDto(this NetAppUser user);
    public static partial IQueryable<RoleResponse> ProjectToRoleResponse(this IQueryable<NetAppRole> source);
    public static partial RoleResponse ToRoleResponse(this NetAppRole role);
}
