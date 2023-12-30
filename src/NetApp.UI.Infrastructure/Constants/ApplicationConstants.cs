namespace NetApp.UI.Infrastructure;
public static class ApplicationConstants
{
    public static class ErrorMessages
    {
        public const string SessionTimeout = "Session timeout";
        public const string ServerError = "An error occurred.";
    }
    public static class Storage
    {
        public const string AuthToken = "axctsuybbx";
        public const string RefreshToken = "xbnchgyiuqij";
        public const string AppState = "xabsgduyiopsb";
    }

    public static class Routes
    {
        public const string Login = "login";
        public const string Home = "/";
        public const string Dashboard = "/";
        public const string Users = "user-management/users";
        public const string UserDetail = "user-management/users/";
        public const string Roles = "user-management/roles";
        public const string Profile = "/profile";
        public const string NotFound = "/404";
        public const string ForgotPassword = "/forgot-password";
        public const string ResetPassword = "/reset-password";
        public const string ConfirmEmail = "/confirm-email";
        public const string ConfirmOtp = "/confirm-otp";
        public const string UserProfile = "/account/user-profile";
        public const string Permissions = "user-management/permissions";
    }

    public static Dictionary<string, (string label, bool valid)> BreadCrumbLabelsAndLinks = new()
    {
        {Routes.Dashboard, ("Dashboard", true)},
        {"users", ("Users", true)},
        {"roles", ("Roles", true)},
        {"permission", ("Account", true)},
        {"user-profile", ("User Profile", true)},
        {"user-management", ("User Management", false)},

    };

}

