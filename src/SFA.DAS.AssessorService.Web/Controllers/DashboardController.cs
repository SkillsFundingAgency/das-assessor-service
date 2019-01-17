using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Client.Exceptions;
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

        public DashboardController(ISessionService sessionService,  
            IHttpContextAccessor contextAccessor , 
            IStandardsApiClient standardsApiClient,
            IOrganisationsApiClient organisationApiClient, 
            ICertificateApiClient certificateApiClieet)
        {
            _sessionService = sessionService;
            _organisationApiClient = organisationApiClient;
            _contextAccessor = contextAccessor;
            _certificateApiClient = certificateApiClieet;
            _standardsApiClient = standardsApiClient;
        }


        [Route("Dashboard")]
        public async Task<IActionResult> Index()
        {
            _sessionService.Set("CurrentPage", Pages.Dashboard);
            var dashboardStatisticsModel = new DashboardStatisticsModel();
            var username = _contextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;
            var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")?.Value;
            try
            {
               var  organisation = await _organisationApiClient.Get(ukprn);
                if (organisation != null)
                {
                    dashboardStatisticsModel.StandardsCount =
                        (await _standardsApiClient.GetEpaoStandardsCount(organisation.EndPointAssessorOrganisationId)).Count;
                    dashboardStatisticsModel.PipelinesCount =
                        (await _standardsApiClient.GetEpaoPipelineCount(organisation.EndPointAssessorOrganisationId))
                        .Count;
                    dashboardStatisticsModel.AssessmentsCount =
                    (await _certificateApiClient.GetCertificatesCount(username)).Count;
                }
                
            }
            catch (EntityNotFoundException)
            {
                return RedirectToAction("NotRegistered", "Home");
            }
            return View(dashboardStatisticsModel);
        }
    }
}