using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Client.Exceptions;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Constants;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Dashboard;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [CheckSession]
    public class DashboardController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOrganisationsApiClient _organisationApiClient;
        private readonly IStandardsApiClient _standardsApiClient;
        private readonly ICertificateApiClient _certificateApiClient;
        private readonly IWebConfiguration _webConfiguration;

        public DashboardController(ISessionService sessionService,  
            IHttpContextAccessor contextAccessor , 
            IStandardsApiClient standardsApiClient,
            IOrganisationsApiClient organisationApiClient, 
            ICertificateApiClient certificateApiClieet,
            IWebConfiguration webConfiguration)
        {
            _sessionService = sessionService;
            _organisationApiClient = organisationApiClient;
            _contextAccessor = contextAccessor;
            _certificateApiClient = certificateApiClieet;
            _standardsApiClient = standardsApiClient;
            _webConfiguration = webConfiguration;
        }


        [Route("Dashboard")]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] {Pages.Dashboard })]
        public async Task<IActionResult> Index()
        {
           
            var dashboardViewModel = new DashboardViewModel($"{_webConfiguration.ApplyBaseAddress}/Applications");
            var username = _contextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;
            var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")?.Value;
            try
            {
               var  organisation = await _organisationApiClient.Get(ukprn);
                if (organisation != null)
                {
                    dashboardViewModel.StandardsCount =
                        (await _standardsApiClient.GetEpaoStandardsCount(organisation.EndPointAssessorOrganisationId)).Count;
                    dashboardViewModel.PipelinesCount =
                        (await _standardsApiClient.GetEpaoPipelineCount(organisation.EndPointAssessorOrganisationId))
                        .Count;
                    dashboardViewModel.AssessmentsCount =
                    (await _certificateApiClient.GetCertificatesCount(username)).Count;
                }
                
            }
            catch (EntityNotFoundException)
            {
                return RedirectToAction("NotRegistered", "Home");
            }
            return View(dashboardViewModel);
        }
    }
}