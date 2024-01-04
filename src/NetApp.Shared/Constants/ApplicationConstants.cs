namespace NetApp.Constants;
public static class SharedConstants
{
    public static class SignalR
    {
        public const string HubUrl = "/hubs";
        public const string OnRolesUpdated = "RolesUpdated";
    }

    public static class CustomClaimTypes
    {
        public const string Permission = "Permission";
        public const string SessionId = "SessionId";
    }
}
