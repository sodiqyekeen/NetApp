using NetApp.Domain.Constants;
using NetApp.Domain.Entities;
using NetApp.Extensions;
using System.Security.Claims;

namespace NetApp.Api.Services
{
    public class SessionService(IHttpContextAccessor httpContextAccessor) : ISessionService
    {
        public Session CurrentSession => httpContextAccessor.HttpContext!.Session.GetString(DomainConstants.SessionKey)!.FromJson<Session>();
        public string Username => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name)??"";

        public string Email => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email)??"";

        public string UserId => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)??"";

        public string Role => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role)??"";
    }
}
