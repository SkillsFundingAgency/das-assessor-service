using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Web.Controllers;

namespace SFA.DAS.AssessorService.Web.Infrastructure
{
    public interface ISessionService
    {
        void Set(string key, object value);
        void Remove(string key);
        string Get(string key);
    }

    class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _environment;

        public SessionService(IHttpContextAccessor httpContextAccessor, string environment)
        {
            _httpContextAccessor = httpContextAccessor;
            _environment = environment;
        }
        
        public void Set(string key, object value)
        {
            _httpContextAccessor.HttpContext.Session.SetString(_environment + "_" + key,
                JsonConvert.SerializeObject(value));
        }

        public void Remove(string key)
        {
            _httpContextAccessor.HttpContext.Session.Remove(_environment + "_" + key);
        }

        public string Get(string key)
        {
            return _httpContextAccessor.HttpContext.Session.GetString(_environment + "_" + key);
        }
    }
}