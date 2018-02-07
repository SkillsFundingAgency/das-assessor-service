using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Web.Services;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    public class OrganisationController : BaseController
    {
        private readonly IOrganisationService _organisationService;

        public OrganisationController(IOrganisationService organisationService, ICache cache) : base(cache)
        {
            _organisationService = organisationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var jwt = GetJwt();

            var organisation = await _organisationService.GetOrganisation(jwt);
            
            return View(organisation);
        }
    }

    public class BaseController : Controller
    {
        private readonly ICache _cache;

        public BaseController(ICache cache)
        {
            _cache = cache;
        }

        protected string GetJwt()
        {
            // This needs to handle the token not being in the cache, the token having expired.
            var userObjectId = (User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier"))?.Value;

            return _cache.GetString(userObjectId);
        }
    }
}