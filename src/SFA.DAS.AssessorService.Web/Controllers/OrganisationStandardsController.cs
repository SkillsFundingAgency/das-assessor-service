﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Exceptions;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AssessorService.Web.Constants;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Models;
using SFA.DAS.AssessorService.Web.StartupConfiguration;
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
        private const int PageSize = 10;

        #region Routes
        public const string OrganisationStandardsIndexGetRoute = nameof(OrganisationStandardsIndexGetRoute);
        #endregion

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
        [Route("/[controller]/", Name = OrganisationStandardsIndexGetRoute)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Standards })]
        public async Task<IActionResult> Index(int? pageIndex)
        {
            var model = new ApprovedStandardsWithVersionsViewModel();
            
            try
            {
                var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
                var organisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);
                if (organisation != null)
                {
                    model.ApprovedStandardsWithVersions = await _standardsApiClient.GetEpaoRegisteredStandards(organisation.OrganisationId, false, pageIndex ?? 1, PageSize);
                    if (organisation.FinancialReviewStatus != ApplyTypes.FinancialReviewStatus.Exempt)
                    {
                        model.FinancialInfoStage1Expired = true;
                        model.FinancialAssessmentUrl = this.Url.Action("StartOrResumeApplication", "Application");
                    }
                }
            }
            catch (EntityNotFoundException)
            {
                return RedirectToAction("NotRegistered", "Home");
            }
            
            return View("Index", model);
        }

        [HttpGet]
        [Route("/[controller]/pipelines")]
        [PrivilegeAuthorize(Privileges.ViewPipeline)]
        public async Task<IActionResult> Pipeline(string selectedStandard, string selectedProvider, string selectedEPADate, int? pageIndex)
        {
            OrderedListResultViewModel orderedListResultViewModel;
            try
            {
                _sessionService.Set("CurrentPage", Pages.Pipeline);
                var orderDirection = TableColumnOrder.None;
                string orderBy = null;

                if (_sessionService.Get("orderDirection") != null && _sessionService.Get("orderBy") != null)
                {
                    orderDirection = _sessionService.Get("orderDirection");
                    orderBy = _sessionService.Get("orderBy");
                }

                orderedListResultViewModel = await GetPipeline(selectedStandard, selectedProvider, selectedEPADate, orderBy, orderDirection, PageSize, pageIndex);
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
            try
            {
                var newOrderdDirection = NextOrderDirection(orderDirection, orderBy);
                orderedListResultViewModel = await GetPipeline(null, null, null, orderBy, newOrderdDirection, PageSize, pageIndex);
            }
            catch (EntityNotFoundException)
            {
                return RedirectToAction("NotRegistered", "Home");
            }

            return View("Pipelines", orderedListResultViewModel);
        }

        [HttpGet]
        [Route("/[controller]/DownloadCsv")]
        public async Task<FileContentResult> ExportEpaPipelineAsCsv(string selectedStandard, string selectedProvider, string selectedEPADate)
        {
            _logger.LogInformation("Starting to download Pipeline EPA CSV File");

            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;

            var organisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);
            var response = await _standardsApiClient.GetEpaoPipelineStandardsExtract(organisation?.OrganisationId, selectedStandard, selectedProvider, selectedEPADate);

            string[] columnHeaders = {
                "Standard Name",
                "Apprentices",
                "UKPRN",
                "Estimated EPA date",
                "Provider Name"
            };

            var piplelineRecords = (from pipeline in response
                select new object[]
                {
                    $"\"{pipeline.StandardName}, {pipeline.StandardVersion}\"",
                    $"\"{pipeline.Pipeline}\"",
                    $"\"{pipeline.ProviderUkPrn}\"",
                    $"\"{pipeline.EstimatedDate}\"",
                    $"\"{pipeline.ProviderName}\"",
                }).ToList();

            var pipelineCsv = new StringBuilder();
            piplelineRecords.ForEach(line =>
            {
                pipelineCsv.AppendLine(string.Join(",", line));
            });
            var buffer = Encoding.ASCII.GetBytes($"{string.Join(",", columnHeaders)}\r\n{pipelineCsv}");
            _logger.LogInformation($"Downloading {buffer.Length} bytes.");
            return File(buffer, "text/csv", $"EpaPipeline.csv");
        }

        private async Task<OrderedListResultViewModel> GetPipeline(string selectedStandard, string selectedProvider, string selectedEPADate, string orderBy, string orderDirection,int pageSize, int? pageIndex)
        {
            var orderedListResultViewModel = new OrderedListResultViewModel
            {
                OrderDirection = string.IsNullOrEmpty(orderDirection) ? TableColumnOrder.None : orderDirection,
                OrderedBy = string.IsNullOrEmpty(orderBy) ? null : orderBy,
                Response = new PaginatedList<EpaoPipelineStandardsResponse>(new List<EpaoPipelineStandardsResponse>(),
                    0, 1, 1)
            };
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;

            var organisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);
            if (organisation != null)
            {
                var filters = await _standardsApiClient.GetEpaoPipelineStandardsFilters(organisation.OrganisationId);

                orderedListResultViewModel.Response =
                    await _standardsApiClient.GetEpaoPipelineStandards(organisation.OrganisationId, selectedStandard, selectedProvider, selectedEPADate,
                        orderBy, orderDirection, pageSize, pageIndex ?? 1);

                InitStandardFilter(orderedListResultViewModel, filters?.StandardFilterItems);
                InitProviderFilter(orderedListResultViewModel, filters?.ProviderFilterItems);
                InitEPADateFilter(orderedListResultViewModel, filters?.EPADateFilterItems);
            }

            if (!string.IsNullOrWhiteSpace(selectedStandard) && selectedStandard.Trim().ToUpper() != "ALL")
            {
                orderedListResultViewModel.SelectedStandard = selectedStandard;
                orderedListResultViewModel.FilterApplied = true;
            }
            if (!string.IsNullOrWhiteSpace(selectedProvider) && selectedProvider.Trim().ToUpper() != "ALL")
            {
                orderedListResultViewModel.SelectedProvider = selectedProvider;
                orderedListResultViewModel.FilterApplied = true;
            }
            if (!string.IsNullOrWhiteSpace(selectedEPADate) && selectedEPADate.Trim().ToUpper() != "ALL")
            {
                orderedListResultViewModel.SelectedEPADate = selectedEPADate;
                orderedListResultViewModel.FilterApplied = true;
            }

            return orderedListResultViewModel;
        }

        private string NextOrderDirection(string sortDirection, string orderBy)
        {
            var newSortDirection = sortDirection == TableColumnOrder.None || sortDirection == TableColumnOrder.Ascending
                ? TableColumnOrder.Descending
                : TableColumnOrder.Ascending;
            _sessionService.Set("orderDirection", newSortDirection);
            _sessionService.Set("orderBy", orderBy);
            return newSortDirection;
        }

        private void InitStandardFilter(OrderedListResultViewModel model, IEnumerable<Domain.Entities.EpaoPipelineStandardFilter> items)
        {
            model.StandardFilter = new List<OrderedListResultViewModel.PipelineFilterItem>() { new OrderedListResultViewModel.PipelineFilterItem() { Id = "ALL", Value = "All standards" }  };

            if(null != items && items.Any())
            {
                model.StandardFilter.AddRange(items.Select(i => new OrderedListResultViewModel.PipelineFilterItem() { Id = i.Id, Value = i.Value }));
            }
        }

        private void InitProviderFilter(OrderedListResultViewModel model, IEnumerable<Domain.Entities.EpaoPipelineStandardFilter> items)
        {
            model.ProviderFilter = new List<OrderedListResultViewModel.PipelineFilterItem>() { new OrderedListResultViewModel.PipelineFilterItem() { Id = "ALL", Value = "All providers" } };

            if (null != items && items.Any())
            {
                model.ProviderFilter.AddRange(items.Select(i => new OrderedListResultViewModel.PipelineFilterItem() { Id = i.Id, Value = i.Value }));
            }
        }

        private void InitEPADateFilter(OrderedListResultViewModel model, IEnumerable<Domain.Entities.EpaoPipelineStandardFilter> items)
        {
            model.EPADateFilter = new List<OrderedListResultViewModel.PipelineFilterItem>() { new OrderedListResultViewModel.PipelineFilterItem() { Id = "ALL", Value = "All dates" } };

            if (null != items && items.Any())
            {
                model.EPADateFilter.AddRange(items.Select(i => new OrderedListResultViewModel.PipelineFilterItem() { Id = i.Id, Value = i.Value }));
            }
        }
   }
}