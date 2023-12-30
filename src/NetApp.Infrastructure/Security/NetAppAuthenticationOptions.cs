using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace NetApp.Infrastructure.Security;

public class NetAppAuthenticationOptions : AuthenticationSchemeOptions
{
}

public static class NetAppAuthenticationDefaults
{
    public const string AuthenticationScheme = "Bearer";
}