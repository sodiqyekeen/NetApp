using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetApp.Domain.Entities;
using NetApp.Domain.Models;
using NetApp.Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace NetApp.Infrastructure.Security;

public class NetAppAuthenticationHandler(IOptionsMonitor<NetAppAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IRepositoryProvider repositoryProvider,
    IDateTimeService dateTimeService,
    IOptions<JwtSettings> jwtSettings,
    IHttpContextAccessor httpContextAccessor) : AuthenticationHandler<NetAppAuthenticationOptions>(options, logger, encoder)
{
    private readonly JwtSettings jwtSettings = jwtSettings.Value;
    private string ErrorMessage { get; set; } = string.Empty;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var token = GetToken();
        if (token is null)
            return AuthenticateResult.NoResult();

        var session = await GetSession(token);
        if (session is null)
        {
            ErrorMessage = "Invalid token";
            return AuthenticateResult.Fail(ErrorMessage);
        }

        try
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var claimPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(session.SigningKey!)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer =jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            if (validatedToken is null || claimPrincipal is null)
            {
                ErrorMessage = "Invalid token";
                return AuthenticateResult.Fail(ErrorMessage);
            }

            var claimsIdentity = new ClaimsIdentity(claimPrincipal.Claims, Scheme.Name);
            //TODO: Build the claims instead of using the the validated claims as is.
            // var user = await userManager.FindByIdAsync(claimPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)!);

            httpContextAccessor.HttpContext!.Session.SetString(DomainConstants.SessionKey, session.ToJson());

            return AuthenticateResult.Success(new AuthenticationTicket(claimPrincipal, Scheme.Name));
        }
        catch
        {
            ErrorMessage = "Invalid token";
            return AuthenticateResult.Fail(ErrorMessage);
        }
        ////Check if the endpoint allows anonymous access
        //var endpoint = Context.GetEndpoint();
        //if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)


    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        ////get the failure reason if any
        //var failure = properties?.GetParameter<string>("error_description");
        //if (string.IsNullOrEmpty(failure))
        //    failure = properties?.GetParameter<string>("error");

        Response.StatusCode = 401;
        Response.ContentType = "application/json";
        Response.Headers.Append("WWW-Authenticate", $"{NetAppAuthenticationDefaults.AuthenticationScheme} realm=\"{ErrorMessage}\"");
        await Context.Response.WriteAsJsonAsync(NetApp.Models.Response.Fail(ErrorMessage));
    }

    protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = 403;
        Response.ContentType = "application/json";
        await Response.WriteAsJsonAsync(NetApp.Models.Response.Fail("Unathorized access denied."));
    }



    private string? GetToken() =>
        Request.Path.StartsWithSegments(SharedConstants.SignalR.HubUrl)
            ? (string?)Request.Query["access_token"]
            : (Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last());

    private async Task<Session?> GetSession(string token)
    {
        var claims = StringExtensions.GetClaims(token);
        var sessionId = claims.FirstOrDefault(c => c.Type == SharedConstants.CustomClaimTypes.SessionId)?.Value;
        if (sessionId is null) return null;
        var session = await repositoryProvider.SessionRepository.GetSessionByIdAsync(Guid.Parse(sessionId));
        if (session is null) return null;
        if (session.ValidUntil > dateTimeService.Now) return session;

        repositoryProvider.SessionRepository.DeleteSession(session);
        await repositoryProvider.SaveChangesAsync();
        return null;
    }
}
