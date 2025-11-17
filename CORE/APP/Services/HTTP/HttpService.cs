using Microsoft.AspNetCore.Http;

namespace CORE.APP.Services.HTTP
{
    public class HttpService : HttpServiceBase
    {
        public HttpService(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory)
            : base(httpContextAccessor, httpClientFactory)
        {
        }
    }
}