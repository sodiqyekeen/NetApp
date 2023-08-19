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
        public const string AuthToken = "athkey";
        public const string RefreshToken = "refkey";
        public const string AppState = "netappstate";
    }

    public static class Routes
    {
        public const string Login = "login";
        public const string Home = "/";
        public const string Dashboard = "dashboard";
        public const string Users = "/users";
        public const string Roles = "/roles";
        public const string Profile = "/profile";
        public const string NotFound = "/404";
    }

}

