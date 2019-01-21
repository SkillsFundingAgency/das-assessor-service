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
using SFA.DAS.AssessorService.Web.ViewModels.OrganisationStandards;

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
            OrderedListResultViewModel orderedListResultViewModel;
            _sessionService.Set("CurrentPage", Pages.Pipeline);
            var orderDirection = "none";
            string orderBy = null;

            if (_sessionService.Get("orderDirection") != null && _sessionService.Get("orderBy") != null)
            {
                orderDirection = _sessionService.Get("orderDirection");
                orderBy = _sessionService.Get("orderBy");
            }

            try
            {
                orderedListResultViewModel = await GetPipeline(orderBy, orderDirection, pageIndex);
            }
            catch (EntityNotFoundException)
            {
                return RedirectToAction("NotRegistered", "Home");
            }

            return View("Pipelines", orderedListResultViewModel);
        }

        [HttpGet]
        [Route("/[controller]/pipeline/{orderBy}/{orderDirection}")]
        public async Task<IActionResult> OrderPipeline(string orderBy, string orderDirection, int? pageIndex)
        {
            OrderedListResultViewModel orderedListResultViewModel;
            var newOrderdDirection = NextOrderDirection(orderDirection, orderBy);
            try
            {
                orderedListResultViewModel = await GetPipeline(orderBy, newOrderdDirection, pageIndex);
            }
            catch (EntityNotFoundException)
            {
                return RedirectToAction("NotRegistered", "Home");
            }

            return View("Pipelines", orderedListResultViewModel);
        }


        private async Task<OrderedListResultViewModel> GetPipeline(string orderBy, string orderDirection, int? pageIndex)
        {
            var orderedListResultViewModel = new OrderedListResultViewModel
            {
                OrderDirection = string.IsNullOrEmpty(orderDirection) ? "none" : orderDirection,
                OrderedBy = string.IsNullOrEmpty(orderBy) ? null : orderBy,
                Response = new PaginatedList<EpaoPipelineStandardsResponse>(new List<EpaoPipelineStandardsResponse>(),
                    0, 1, 1)
            };
            var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")?.Value;

            var organisation = await _organisationsApiClient.Get(ukprn);
            if (organisation != null)
            {
                orderedListResultViewModel.Response =
                    await _standardsApiClient.GetEpaoPipelineStandards(organisation.EndPointAssessorOrganisationId,
                        orderBy, orderDirection, pageIndex ?? 1);
            }

            return orderedListResultViewModel;
        }

        private string NextOrderDirection(string sortDirection, string orderBy)
        {
            var newSortDirection = sortDirection == "none" || sortDirection == "ascending" ? "descending" : "ascending";
            _sessionService.Set("orderDirection", newSortDirection);
            _sessionService.Set("orderBy", orderBy);
            return newSortDirection;
        }
    }
}