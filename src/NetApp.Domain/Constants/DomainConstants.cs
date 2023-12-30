namespace NetApp.Domain.Constants;

public static class DomainConstants
{
    public const string SessionKey = "CurrentSession";

    public static class Role
    {
        public const string SuperAdmin = "SUPERADMIN";
        public const string Basic = "BASIC";
        public const string Admin = "ADMIN";
    }

    public static class EmailTemplate
    {
        public const string Welcome = "WELCOME";
        public const string ResetPassword = "RESETPASSWORD";
        public const string ConfirmEmail = "CONFIRMEMAIL";
    }
}
