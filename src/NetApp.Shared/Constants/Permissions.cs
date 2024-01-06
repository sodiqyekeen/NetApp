
using System.ComponentModel;

namespace NetApp.Shared.Constants;

public static class Permissions
{
    [DisplayName("User"), Description("User Permissions")]
    public static class User
    {
        [Description("Can view users.")]
        public const string View = "Permissions.User.View";

        [Description("Can create users.")]
        public const string Create = "Permissions.User.Create";

        [Description("Can edit users.")]
        public const string Edit = "Permissions.User.Edit";

        [Description("Can delete users.")]
        public const string Delete = "Permissions.User.Delete";

        [Description("Can manage user permission.")]
        public const string ManageUserPermission = "Permissions.User.ManagePermission";

        [Description("Can manage user roles.")]
        public const string ManageRole = "Permissions.User.ManageRole";

        [Description("Can resend user confirmation.")]
        public const string ResendConfirmation = "Permissions.User.ResendConfirmation";

        [Description("Can view user details.")]
        public const string ViewDetails = "Permissions.User.ViewDetails";

        [Description("Can export users.")]
        public const string Export = "Permissions.User.Export";

        [Description("Can view user roles.")]
        public const string ViewRoles = "Permissions.User.ViewRoles";
    }

    [DisplayName("Role"), Description("Role Permissions")]
    public static class Role
    {
        [Description("Can view roles.")]
        public const string View = "Permissions.Role.View";

        [Description("Can create roles.")]
        public const string Create = "Permissions.Role.Create";

        [Description("Can edit roles.")]
        public const string Edit = "Permissions.Role.Edit";

        [Description("Can delete roles.")]
        public const string Delete = "Permissions.Role.Delete";

        [Description("Can manage role permission.")]
        public const string ManageRolePermission = "Permissions.Role.ManagePermission";
    }

    [DisplayName("Permission"), Description("System Permissions")]
    public static class Permission
    {
        [Description("Can view permissions.")]
        public const string View = "Permissions.RoleClaim.View";

        [Description("Can manage permissions.")]
        public const string Manage = "Permissions.RoleClaim.Manage";
    }
}