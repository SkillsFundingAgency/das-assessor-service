using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.Api.Types.Exceptions;
using SFA.DAS.AssessorService.Api.Types;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AssessorService.Web.Constants;
using SFA.DAS.AssessorService.Web.Infrastructure;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [CheckSession]
    public class OrganisationStandardsController : Controller
    {
        private readonly ILogger<OrganisationStandardsController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ISessionService _sessionService;
        private readonly IOrganisationsApiClient _organisationsApiClient;
        private readonly IStandardsApiClient _standardsApiClient;

        public OrganisationStandardsController(ILogger<OrganisationStandardsController> logger, 
            ISessionService sessionService,
            IOrganisationsApiClient organisationsApiClient,
            IStandardsApiClient standardsApiClient,
            IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _sessionService = sessionService;
            _organisationsApiClient = organisationsApiClient;
            _contextAccessor = contextAccessor;
            _standardsApiClient = standardsApiClient;
        }

        [HttpGet]
        [Route("/[controller]/")]
        public async Task<IActionResult> Index(int? pageIndex)
        {
            _sessionService.Set("CurrentPage", Pages.Standards);
            var epaoRegisteredStandardsResponse = new PaginatedList<GetEpaoRegisteredStandardsResponse>( new List<GetEpaoRegisteredStandardsResponse>(),0,1,1);

            var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")?.Value;
            try
            {
               
                var organisation = await _organisationsApiClient.Get(ukprn);
                if (organisation != null)
                {
                    epaoRegisteredStandardsResponse = await _standardsApiClient.GetEpaoRegisteredStandards(organisation.EndPointAssessorOrganisationId, pageIndex ?? 1);
                }

            }
            catch (EntityNotFoundException)
            {
                return RedirectToAction("NotRegistered", "Home");
            }
            
            return View("Index", epaoRegisteredStandardsResponse);
        }

        [HttpGet]
        [Route("/[controller]/pipelines")]
        public async Task<IActionResult> Pipeline(int? pageIndex)
        {
            _sessionService.Set("CurrentPage", Pages.Pipeline);
            var epaoPipelineStandardsResponse = new PaginatedList<GetEpaoPipelineStandardsResponse>(new List<GetEpaoPipelineStandardsResponse>(), 0, 1, 1);

            var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")?.Value;
            try
            {

                var organisation = await _organisationsApiClient.Get(ukprn);
                if (organisation != null)
                {
                    epaoPipelineStandardsResponse = await _standardsApiClient.GetEpaoPipelineStandards(organisation.EndPointAssessorOrganisationId, pageIndex ?? 1);
                }

            }
            catch (EntityNotFoundException)
            {
                return RedirectToAction("NotRegistered", "Home");
            }

            return View("Pipelines", epaoPipelineStandardsResponse);
        }
    }
}