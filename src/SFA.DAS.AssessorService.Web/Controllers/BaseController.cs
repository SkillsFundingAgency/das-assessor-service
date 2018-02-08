using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Web.Services;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    public class BaseController : Controller
    {
        private readonly ICache _cache;
        private readonly IHttpContextAccessor _contextAccessor;

        public BaseController(ICache cache, IHttpContextAccessor contextAccessor)
        {
            _cache = cache;
            _contextAccessor = contextAccessor;
        }

        protected string GetJwt()
        {
            // This needs to handle the token not being in the cache, the token having expired.
            var userObjectId = _contextAccessor.HttpContext.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
            
            return _cache.GetString(userObjectId);
        }
    }
}