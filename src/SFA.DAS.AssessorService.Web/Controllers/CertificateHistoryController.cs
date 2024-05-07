using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Constants;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.StartupConfiguration;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [PrivilegeAuthorize(Privileges.ViewCompletedAssessments)]
    [CheckSession(CertificateHistoryRoute, nameof(ResetSession), nameof(ICertificateHistorySession.CertificateHistorySearchTerm))]
    [Route(CertificateHistoryRoute)]
    public class CertificateHistoryController : Controller
    {
        private readonly ILogger<CertificateController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICertificateApiClient _certificateApiClient;
        private readonly ICertificateHistorySession _certificateHistorySession;
        private const string CertificateHistoryRoute = "certificateHistory";
        private const int DefaultPageIndex = 1;

        public CertificateHistoryController(
            ILogger<CertificateController> logger,
            IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient,
            ICertificateHistorySession certificateHistorySession)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _certificateApiClient = certificateApiClient;
            _certificateHistorySession = certificateHistorySession;
        }

        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Assessments })]
        public async Task<IActionResult> Index()
        {
            SetDefaultSession();
            var vm = await MapViewModelFromSession();
            return View(nameof(Index), vm);
        }

        [HttpGet(nameof(ResetSession))]
        [CheckSession(nameof(ICertificateHistorySession.CertificateHistorySearchTerm), CheckSession.Ignore)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Assessments })]
        public IActionResult ResetSession()
        {
            SetDefaultSession();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet(nameof(Search))]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Assessments })]
        public async Task<IActionResult> Search(string searchTerm)
        {
            _certificateHistorySession.CertificateHistorySearchTerm = searchTerm?.Trim() ?? string.Empty;

            // reset the page indexes as the new results may have less pages
            _certificateHistorySession.CertificateHistoryPageIndex = DefaultPageIndex;

            var vm = await MapViewModelFromSession();
            return View(nameof(Index), vm);
        }

        [HttpGet(nameof(SortCertificateHistory))]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Assessments })]
        public async Task<IActionResult> SortCertificateHistory(string sortColumn, string sortDirection)
        {
            UpdateCertificateHistorySortDirection(sortColumn, sortDirection);
            return await ChangePageCertificateHistory();
        }

        [HttpGet(nameof(ChangePageCertificateHistory))]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Assessments })]
        public async Task<IActionResult> ChangePageCertificateHistory(int pageIndex = DefaultPageIndex)
        {
            _certificateHistorySession.CertificateHistoryPageIndex = pageIndex;

            var vm = await MapViewModelFromSession();
            return View(nameof(Index), vm);
        }

        private void UpdateCertificateHistorySortDirection(string sortColumnName, string sortDirection)
        {
            if (Enum.TryParse(sortColumnName, true, out GetCertificateHistoryRequest.SortColumns sortColumn))
            {
                if (_certificateHistorySession.CertificateHistorySortColumn == sortColumn)
                {
                    _certificateHistorySession.CertificateHistorySortDirection = sortDirection;
                }
                else
                {
                    _certificateHistorySession.CertificateHistorySortColumn = sortColumn;
                    _certificateHistorySession.CertificateHistorySortDirection = "Asc";
                }
            }
        }

        private async Task<CertificateHistoryViewModel> MapViewModelFromSession()
        {
            var viewModel = new CertificateHistoryViewModel();

            viewModel.SearchTerm = _certificateHistorySession.CertificateHistorySearchTerm;
            viewModel.SortColumn = _certificateHistorySession.CertificateHistorySortColumn;
            viewModel.SortDirection = _certificateHistorySession.CertificateHistorySortDirection;
            viewModel.PageIndex = _certificateHistorySession.CertificateHistoryPageIndex;
            var endPointAssessorOrganisationId = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;

            viewModel.Certificates = await _certificateApiClient.GetCertificateHistory
                    (viewModel.PageIndex ?? 1,
                    endPointAssessorOrganisationId,
                    viewModel.SearchTerm,
                    viewModel.SortColumn.ToString(),
                    viewModel.SortDirection == "Asc" ? 1 : 0);

            return viewModel;
        }

        private void SetDefaultSession()
        {
            _certificateHistorySession.CertificateHistorySearchTerm = string.Empty;
            _certificateHistorySession.CertificateHistorySortColumn = GetCertificateHistoryRequest.SortColumns.DateRequested;
            _certificateHistorySession.CertificateHistoryPageIndex = DefaultPageIndex;
            _certificateHistorySession.CertificateHistorySortDirection = "Desc";
        }
    }
}