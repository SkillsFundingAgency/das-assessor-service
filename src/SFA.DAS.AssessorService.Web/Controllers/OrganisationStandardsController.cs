using System.Collections.Generic;
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
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Standards })]
        public async Task<IActionResult> Index(int? pageIndex)
        {
            var epaoRegisteredStandardsResponse =
                new PaginatedList<GetEpaoRegisteredStandardsResponse>(new List<GetEpaoRegisteredStandardsResponse>(), 0,
                    1, 1);
            try
            {
                var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
                var organisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);
                if (organisation != null)
                    epaoRegisteredStandardsResponse =
                        await _standardsApiClient.GetEpaoRegisteredStandards(
                            organisation.OrganisationId, pageIndex ?? 1, 10);
            }
            catch (EntityNotFoundException)
            {
                return RedirectToAction("NotRegistered", "Home");
            }

            return View("Index", epaoRegisteredStandardsResponse);
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

                orderedListResultViewModel = await GetPipeline(orderBy, orderDirection, PageSize, pageIndex);
                orderedListResultViewModel.SelectedStandard = selectedStandard;
                orderedListResultViewModel.SelectedProvider = selectedProvider;
                orderedListResultViewModel.SelectedEPADate = selectedEPADate;
                ApplyFilters(orderedListResultViewModel);
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
                orderedListResultViewModel = await GetPipeline(orderBy, newOrderdDirection, PageSize, pageIndex);
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
            var response = await _standardsApiClient.GetEpaoPipelineStandardsExtract(organisation?.OrganisationId);

            ApplyFilters(ref response, selectedStandard, selectedProvider, selectedEPADate);

            string[] columnHeaders = {
                "Standard Name",
                "Apprentices",
                "UKPRN",
                "Estimated EPA date"
            };

            var piplelineRecords = (from pipeline in response
                select new object[]
                {
                    $"{pipeline.StandardName}",
                    $"\"{pipeline.Pipeline}\"",
                    $"\"{pipeline.ProviderUkPrn}\"",
                    $"\"{pipeline.EstimatedDate}\"",
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

        private async Task<OrderedListResultViewModel> GetPipeline(string orderBy, string orderDirection,int pageSize, int? pageIndex)
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
                orderedListResultViewModel.Response =
                    await _standardsApiClient.GetEpaoPipelineStandards(organisation.OrganisationId,
                        orderBy, orderDirection, pageSize, pageIndex ?? 1);
                InitStandardFilter(orderedListResultViewModel);
                InitProviderFilter(orderedListResultViewModel);
                InitEPADateFilter(orderedListResultViewModel);
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

        private void InitStandardFilter(OrderedListResultViewModel model)
        {
            model.StandardFilter = new List<OrderedListResultViewModel.StandardFilterItem>() { new OrderedListResultViewModel.StandardFilterItem() { Id = "ALL", StandardName = "All standards" }  };

            if(null == model || null == model.Response || null == model.Response.Items || !model.Response.Items.Any())
            {
                return;
            }

            var distinctStandards = model.Response.Items.Select(i => new OrderedListResultViewModel.StandardFilterItem() { Id = i.StandardName, StandardName = i.StandardName }).Distinct();
            model.StandardFilter.AddRange(distinctStandards);
        }
        private void InitProviderFilter(OrderedListResultViewModel model)
        {
            model.ProviderFilter = new List<OrderedListResultViewModel.ProviderFilterItem>() { new OrderedListResultViewModel.ProviderFilterItem() { Id = "ALL", ProviderName = "All providers" } };

            if (null == model || null == model.Response || null == model.Response.Items || !model.Response.Items.Any())
            {
                return;
            }

            // @ToDo once the provider names are coming through from the API
            //var distinctProviders = model.Response.Items.Select(i => new OrderedListResultViewModel.ProviderFilterItem() { Id = i.StandardName, ProviderName = i.StandardName }).Distinct();
            //model.ProviderFilter.AddRange(distinctProviders);
        }

        private void InitEPADateFilter(OrderedListResultViewModel model)
        {
            model.EPADateFilter = new List<OrderedListResultViewModel.EPADateFilterItem>() { new OrderedListResultViewModel.EPADateFilterItem() { Id = "ALL", EPADate = "All dates" } };

            if (null == model || null == model.Response || null == model.Response.Items || !model.Response.Items.Any())
            {
                return;
            }

            var distinctDates = model.Response.Items.Select(i => i.EstimatedDate).Distinct().Select(d => new OrderedListResultViewModel.EPADateFilterItem() { Id = d, EPADate = d });
            model.EPADateFilter.AddRange(distinctDates);
        }

        private void ApplyFilters(OrderedListResultViewModel model)
        {
            if (null == model || null == model.Response || null == model.Response.Items || !model.Response.Items.Any())
            {
                return;
            }
            ApplyStandardFilter(model);
            ApplyProviderFilter(model);
            ApplyEPADateFilter(model);
        }

        private void ApplyStandardFilter(OrderedListResultViewModel model)
        {
            if (null == model || null == model.Response || null == model.Response.Items || !model.Response.Items.Any())
            {
                return;
            }

            // @ToDo: once the ILR data is replaced by Learner this should be matching on Standard Reference - matching on name is too brittle.
            // @ToDo: test that paging is working correctly with filtering.
            if (!string.IsNullOrWhiteSpace(model.SelectedStandard) && model.SelectedStandard.Trim().ToUpper() != "ALL")
            {
                var filteredItems = model.Response.Items.Where(i => i.StandardName == model.SelectedStandard).ToList();
                model.Response = new PaginatedList<EpaoPipelineStandardsResponse>(filteredItems,filteredItems.Count, 0, model.Response.PageSize);
                model.FilterApplied = true;
            }
        }

        private void ApplyProviderFilter(OrderedListResultViewModel model)
        {
            if (null == model || null == model.Response || null == model.Response.Items || !model.Response.Items.Any())
            {
                return;
            }

            // @ToDo: needs the ILR data
            // @ToDo: test that paging is working correctly with filtering.
            if (!string.IsNullOrWhiteSpace(model.SelectedProvider) && model.SelectedStandard.Trim().ToUpper() != "ALL")
            {
                var filteredItems = model.Response.Items/*.Where(i => )*/.ToList();
                model.Response = new PaginatedList<EpaoPipelineStandardsResponse>(filteredItems, filteredItems.Count, 0, model.Response.PageSize);
                model.FilterApplied = true;
            }
        }

        private void ApplyEPADateFilter(OrderedListResultViewModel model)
        {
            if (null == model || null == model.Response || null == model.Response.Items || !model.Response.Items.Any())
            {
                return;
            }

            // @ToDo: needs the ILR data
            // @ToDo: test that paging is working correctly with filtering.
            if (!string.IsNullOrWhiteSpace(model.SelectedEPADate) && model.SelectedEPADate.Trim().ToUpper() != "ALL")
            {
                var filteredItems = model.Response.Items/*.Where(i => )*/.ToList();
                model.Response = new PaginatedList<EpaoPipelineStandardsResponse>(filteredItems, filteredItems.Count, 0, model.Response.PageSize);
                model.FilterApplied = true;
            }
        }

        private void ApplyFilters(ref List<EpaoPipelineStandardsExtractResponse> response, string selectedStandard, string selectedProvider, string selectedEPADate)
        {
            if (null == response || !response.Any())
            {
                return;
            }
            ApplyStandardFilter(ref response, selectedStandard);
            ApplyProviderFilter(ref response, selectedProvider);
            ApplyEPADateFilter(ref response, selectedEPADate);
        }

        private void ApplyStandardFilter(ref List<EpaoPipelineStandardsExtractResponse> response, string selectedStandard)
        {
            if (null == response || !response.Any() || string.IsNullOrWhiteSpace(selectedStandard) || selectedStandard.Trim().ToUpper() == "ALL")
            {
                return;
            }

            // @ToDo: once the ILR data is replaced by Learner this should be matching on Standard Reference - matching on name is too brittle.
            response = response.Where(i => i.StandardName == selectedStandard).ToList();
        }

        private void ApplyProviderFilter(ref List<EpaoPipelineStandardsExtractResponse> response, string selectedProvider)
        {
            if (null == response || !response.Any() || string.IsNullOrWhiteSpace(selectedProvider) || selectedProvider.Trim().ToUpper() == "ALL")
            {
                return;
            }

            // @ToDo: once the ILR data is replaced by Learner this should match provider id
            //response = response.Where(i => ).ToList();
        }

        private void ApplyEPADateFilter(ref List<EpaoPipelineStandardsExtractResponse> response, string selectedEPADate)
        {
            if (null == response || !response.Any() || string.IsNullOrWhiteSpace(selectedEPADate) || selectedEPADate.Trim().ToUpper() == "ALL")
            {
                return;
            }

            response = response.Where(i => i.EstimatedDate == selectedEPADate).ToList();
        }
    }
}