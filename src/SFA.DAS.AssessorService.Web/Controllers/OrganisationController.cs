using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Services;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    public class OrganisationController : Controller
    {
        private readonly IOrganisationService _organisationService;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _contextAccessor;

        public OrganisationController(
            IOrganisationService organisationService, 
            ILogger<OrganisationController> logger, 
            ITokenService tokenService, IHttpContextAccessor contextAccessor)
        {
            _organisationService = organisationService;
            _tokenService = tokenService;
            _contextAccessor = contextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn").Value;
            var jwt = _tokenService.GetJwt();

            var organisation = await _organisationService.GetOrganisation(jwt, int.Parse(ukprn));

            if (organisation != null)
                return View(organisation);

            return RedirectToAction("NotRegistered", "Home");
        }
    }
}