namespace NetApp.UI.Infrastructure.Constants;

public static class Endpoints
{
    public static class Identity
    {
        private const string BaseUrl = "identity/";
        public const string Login = BaseUrl + "login";
        public const string RefreshToken = BaseUrl + "refresh-token";
        public const string Users = BaseUrl + "users";
        public static string UserById(string id) => $"{Users}/{id}";
        public static string ConfirmEmail(string userId, string code) => $"{Users}/{userId}/confirm-email?code={code}";
        public static string ResendConfirmationEmail(string userId) => $"{Users}/{userId}/resend-confirmation-email";
        public const string ForgotPassword = $"{Users}/forgot-password";
        public static string ResetPassword(string userId) => $"{Users}/{userId}/reset-password";
        public static string UpdatePassword(string userId) => $"{Users}/{userId}/update-password";
        public static string UserRoles(string userId) => $"{Users}/{userId}/roles";
    }

    public static class Role
    {
        private const string BaseUrl = "roles/";
        public const string GetAllRolesWithPermissions = BaseUrl + "permissions";
    }
}