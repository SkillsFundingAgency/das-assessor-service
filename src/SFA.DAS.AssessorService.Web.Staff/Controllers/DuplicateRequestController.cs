using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    [Authorize(Policy = Startup.Policies.OperationsTeamOnly)]
    public class DuplicateRequestController : Controller
    {
        private readonly ApiClient _apiClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public DuplicateRequestController(ApiClient apiClient,
            IHttpContextAccessor contextAccessor)
        {
            _apiClient = apiClient;
            _contextAccessor = contextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid certificateId)
        {
            var certificate = await _apiClient.GetCertificate(certificateId);

            var vm = new DuplicateRequestViewModel
            {
                Certificate = certificate,
                IsConfirmed = false,
                NextBatchDate = "Fake Date"
            };

            return View(vm);
        }

        [HttpPost(Name = "Index")]
        public async Task<IActionResult> Index(DuplicateRequestViewModel duplicateRequestViewModel)
        {
            var username = _contextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;
            var certificate = await _apiClient.PostReprintRequest(new StaffCertificateDuplicateRequest
            {
                Id = duplicateRequestViewModel.Certificate.Id,
                Username = username
            });

            var nextScheduledRun = await _apiClient.GetNextScheduledRun((int)ScheduleType.PrintRun);
            var vm = new DuplicateRequestViewModel
            {
                Certificate = certificate,
                IsConfirmed = true,
                NextBatchDate = nextScheduledRun?.RunTime.ToString("dd/MM/yyyy")
            };

            return View(vm);
        }
    }

    public class DuplicateRequestViewModel
    {
        public Certificate Certificate { get; set; }
        public bool IsConfirmed { get; set; }
        public string NextBatchDate { get; set; }
    }
}