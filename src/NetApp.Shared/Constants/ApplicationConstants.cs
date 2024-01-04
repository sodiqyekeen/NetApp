namespace NetApp.Constants;
public static class SharedConstants
{
    public static class SignalR
    {
        public const string HubUrl = "/hubs";
        public const string OnRolesUpdated = "RolesUpdated";
        public const string OnRoleDeleted = "RoleDeleted";
        public const string OnConnected = "Connected";
        public const string NotifyRolesUpdated = "NotifyRolesUpdated";
    }

    public static class CustomClaimTypes
    {
        public const string Permission = "Permission";
        public const string SessionId = "SessionId";
    }
}
