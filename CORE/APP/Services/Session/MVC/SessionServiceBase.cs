using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace CORE.APP.Services.Session.MVC;

public abstract class SessionServiceBase
    {
        protected IHttpContextAccessor HttpContextAccessor { get; }

        protected SessionServiceBase(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }
        
        public virtual T GetSession<T>(string key) where T : class
        {
            var value = HttpContextAccessor.HttpContext.Session.GetString(key);
            if (string.IsNullOrEmpty(value))
                return null;
            return JsonSerializer.Deserialize<T>(value); // Converts JSON string to object of type T
        }

        public virtual void SetSession<T>(string key, T instance) where T : class
        {
            if (instance is not null)
            {
                var value = JsonSerializer.Serialize(instance); // Converts object of type T to JSON string
                HttpContextAccessor.HttpContext.Session.SetString(key, value);
            }
        }

        public virtual void RemoveSession(string key)
        {
            HttpContextAccessor.HttpContext.Session.Remove(key);
        }
    }