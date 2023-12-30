
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
        public const string ManagePermission = "Permissions.User.ManagePermission";
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
    }

    [DisplayName("RoleClaim"), Description("RoleClaim Permissions")]
    public static class RoleClaim
    {
        [Description("Can view role claims.")]
        public const string View = "Permissions.RoleClaim.View";

        [Description("Can create role claims.")]
        public const string Create = "Permissions.RoleClaim.Create";

        [Description("Can edit role claims.")]
        public const string Edit = "Permissions.RoleClaim.Edit";

        [Description("Can delete role claims.")]
        public const string Delete = "Permissions.RoleClaim.Delete";
    }
}