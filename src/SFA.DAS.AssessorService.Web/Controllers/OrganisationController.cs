using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    public class OrganisationController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOrganisationsApiClient _apiClient;

        public OrganisationController(ILogger<OrganisationController> logger, IHttpContextAccessor contextAccessor, IOrganisationsApiClient apiClient)
        {
            _contextAccessor = contextAccessor;
            _apiClient = apiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn").Value;
            
            var organisation = await _apiClient.Get(int.Parse(ukprn));

            if (organisation != null)
                return View(organisation);

            return RedirectToAction("NotRegistered", "Home");
        }
    }
}