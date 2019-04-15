using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
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
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOrganisationsApiClient _organisationApiClient;
        private readonly IStandardsApiClient _standardsApiClient;
        private readonly ICertificateApiClient _certificateApiClient;
        private readonly IWebConfiguration _webConfiguration;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            IHttpContextAccessor contextAccessor , 
            IStandardsApiClient standardsApiClient,
            IOrganisationsApiClient organisationApiClient, 
            ICertificateApiClient certificateApiClieet,
            IWebConfiguration webConfiguration, ILogger<DashboardController> logger)
        {
            _organisationApiClient = organisationApiClient;
            _contextAccessor = contextAccessor;
            _certificateApiClient = certificateApiClieet;
            _standardsApiClient = standardsApiClient;
            _webConfiguration = webConfiguration;
            _logger = logger;
        }

        [HttpGet]
        [Route("Dashboard")]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] {Pages.Dashboard })]
        public async Task<IActionResult> Index()
        {
           
            var dashboardViewModel = new DashboardViewModel($"{_webConfiguration.ApplyBaseAddress}/Applications");
            var username = _contextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;

            try
            {
                if (!string.IsNullOrEmpty(epaoid))
                {
                    var organisation = await _organisationApiClient.GetEpaOrganisation(epaoid);
                    if (organisation != null)
                    {
                        try
                        {
                            dashboardViewModel.StandardsCount = await _standardsApiClient.GetEpaoStandardsCount(epaoid);
                            dashboardViewModel.AssessmentsCount = await _certificateApiClient.GetCertificatesCount(username);
                            dashboardViewModel.PipelinesCount = await _standardsApiClient.GetEpaoPipelineCount(epaoid); 
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Dashboard.Index Counts Exception: {ex.Message} : {ex.StackTrace}");
                        }
                    }
                }

            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation($"Dashboard.Index EntityNotFoundException: {ex.Message} : {ex.StackTrace}");
                return RedirectToAction("NotRegistered", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Dashboard.Index Exception: {ex.Message} : {ex.StackTrace}");
            }
            return View(dashboardViewModel);
        }
    }
}