using Microsoft.AspNetCore.Http;

namespace SFA.DAS.AssessorService.Web.Services
{
    public class SessionCache : ICache
    {
        private readonly IHttpContextAccessor _context;

        public SessionCache(IHttpContextAccessor context)
        {
            _context = context;
        }

        public string GetString(string key)
        {
            return _context.HttpContext.Session.GetString(key);
        }

        public void SetString(string key, string value)
        {
            _context.HttpContext.Session.SetString(key, value);
        }
    }
}