using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Web.Services;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    public class BaseController : Controller
    {
        private readonly ICache _cache;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger _logger;

        public BaseController(ICache cache, IHttpContextAccessor contextAccessor, ILogger logger)
        {
            _cache = cache;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        protected string GetJwt()
        {
            // This needs to handle the token not being in the cache, the token having expired.
            var userObjectId = _contextAccessor.HttpContext.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
            
            _logger.LogInformation("User Id received");

            var jwt = _cache.GetString(userObjectId);

            _logger.LogInformation("JWT received from cache");

            return jwt;
        }
    }
}