using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

        public OrganisationController(
            IOrganisationService organisationService, 
            ILogger<OrganisationController> logger, 
            ITokenService tokenService)
        {
            _organisationService = organisationService;
            _tokenService = tokenService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var ukprn = User.FindFirst("http://schemas.portal.com/ukprn").Value;
            var jwt = _tokenService.GetJwt();

            var organisation = await _organisationService.GetOrganisation(jwt, int.Parse(ukprn));
            
            return View(organisation);
        }
    }
}