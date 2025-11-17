using System.Net.Http.Json;
using CORE.APP.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;

namespace CORE.APP.Services.HTTP;

 public abstract class HttpServiceBase : ServiceBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServiceBase"/> class with the specified HTTP context accessor and HTTP client factory.
        /// </summary>
        /// <param name="httpContextAccessor">Provides access to the current HTTP context.</param>
        /// <param name="httpClientFactory">Factory for creating <see cref="HttpClient"/> instances.</param>
        protected HttpServiceBase(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Creates and configures an <see cref="HttpClient"/> instance.
        /// If an authorization token is present in the current HTTP context, it is added to the request headers.
        /// </summary>
        /// <returns>A configured <see cref="HttpClient"/> instance.</returns>
        protected virtual HttpClient CreateHttpClient()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(token))
            {
                if (token.StartsWith(JwtBearerDefaults.AuthenticationScheme))
                    token = token.Remove(0, JwtBearerDefaults.AuthenticationScheme.Length).TrimStart();
                httpClient.DefaultRequestHeaders.Add("Authorization", token);
            }
            return httpClient;
        }
        
        public virtual async Task<List<TResponse>> GetFromJson<TResponse>(string url, CancellationToken cancellationToken = default)
            where TResponse : Response, new()
        {
            var httpClient = CreateHttpClient();
            var list = await httpClient.GetFromJsonAsync<List<TResponse>>(url, cancellationToken);
            return list;
        }


        public virtual async Task<TResponse> GetFromJson<TResponse>(string url, int id, CancellationToken cancellationToken = default)
            where TResponse : Response, new()
        {
            var httpClient = CreateHttpClient();
            var item = await httpClient.GetFromJsonAsync<TResponse>($"{url}/{id}", cancellationToken);
            return item;
        }

        public virtual async Task<CommandResponse> PostAsJson<TRequest>(string url, TRequest request, CancellationToken cancellationToken = default)
            where TRequest : Request, new()
        {
            var httpClient = CreateHttpClient();
            var response = await httpClient.PostAsJsonAsync(url, request, cancellationToken);
            return response.IsSuccessStatusCode ? Success($"JSON post at {url} successful.", 0) : Error($"JSON post at {url} failed!");
        }
        
        public virtual async Task<CommandResponse> PutAsJson<TRequest>(string url, TRequest request, CancellationToken cancellationToken = default)
            where TRequest : Request, new()
        {
            var httpClient = CreateHttpClient();
            var response = await httpClient.PutAsJsonAsync(url, request, cancellationToken);
            return response.IsSuccessStatusCode ? Success($"JSON put at {url} successful.", request.Id) : Error($"JSON put at {url} failed!");
        }
        
        public virtual async Task<CommandResponse> Delete(string url, int id, CancellationToken cancellationToken = default)
        {
            var httpClient = CreateHttpClient();
            var response = await httpClient.DeleteAsync($"{url}/{id}", cancellationToken);
            return response.IsSuccessStatusCode ? Success($"Delete at {url} successful.", id) : Error($"Delete at {url} failed!");
        }
    }