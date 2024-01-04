using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using NetApp.Shared.Constants;

namespace NetApp.UI.Infrastructure.Extensions;

public static class PermissionProvider
{
    public static bool CanViewUsers(this AuthenticationState authenticationState) =>
        authenticationState.Authorize(Permissions.User.View);

    public static bool CanCreateUser(this AuthenticationState authenticationState) =>
        authenticationState.Authorize(Permissions.User.Create);

    public static bool CanEditUser(this AuthenticationState authenticationState) =>
        authenticationState.Authorize(Permissions.User.Edit);

    public static bool CanDeleteUser(this AuthenticationState authenticationState) =>
        authenticationState.Authorize(Permissions.User.Delete);

    public static bool CanManageUserPermission(this AuthenticationState authenticationState) =>
        authenticationState.Authorize(Permissions.User.ManageUserPermission);

    public static bool CanManageRole(this AuthenticationState authenticationState) =>
        authenticationState.Authorize(Permissions.User.ManageRole);

    public static bool CanResendConfirmation(this AuthenticationState authenticationState) =>
        authenticationState.Authorize(Permissions.User.ResendConfirmation);

    public static bool CanViewUserDetails(this AuthenticationState authenticationState) =>
        authenticationState.Authorize(Permissions.User.ViewDetails);

    public static bool CanExportUsers(this AuthenticationState authenticationState) =>
        authenticationState.Authorize(Permissions.User.Export);

    public static bool CanViewRoles(this AuthenticationState authenticationState) =>
        authenticationState.Authorize(Permissions.Role.View);

    public static bool CanCreateRole(this AuthenticationState authenticationState) =>
        authenticationState.Authorize(Permissions.Role.Create);

    public static bool CanEditRole(this AuthenticationState authenticationState) =>
        authenticationState.Authorize(Permissions.Role.Edit);

    public static bool CanDeleteRole(this AuthenticationState authenticationState) =>
        authenticationState.Authorize(Permissions.Role.Delete);

    public static bool CanManageRolePermission(this AuthenticationState authenticationState) =>
        authenticationState.Authorize(Permissions.Role.ManageRolePermission);

    public static bool CanViewPermissions(this AuthenticationState authenticationState) =>
        authenticationState.Authorize(Permissions.Permission.View);

    public static bool CanManagePermissions(this AuthenticationState authenticationState) =>
        authenticationState.Authorize(Permissions.Permission.Manage);

}