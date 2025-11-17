using Microsoft.AspNetCore.Http;

namespace CORE.APP.Services.Session.MVC
{
    public class SessionService : SessionServiceBase
    {
        public SessionService(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }
    }
}