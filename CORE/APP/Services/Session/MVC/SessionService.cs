using Microsoft.AspNetCore.Http;

namespace CORE.APP.Services.Session.MVC;

public class SessionService : SessionServiceBase
{
    /// <summary>
    /// Initializes a new instance of SessionService with the specified IHttpContextAccessor.
    /// Passes the accessor to the base class to enable session operations.
    /// </summary>
    /// <param name="httpContextAccessor">Accessor for the current HTTP context.</param>
    public SessionService(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
    }
}