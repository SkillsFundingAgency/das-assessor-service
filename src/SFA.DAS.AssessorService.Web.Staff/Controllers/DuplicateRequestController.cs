using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    [Authorize(Roles = Domain.Roles.OperationsTeam + "," + Domain.Roles.CertificationTeam)]
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
        public async Task<IActionResult> Index(
            Guid certificateId,
            int stdCode,
            long uln,
            string searchString,
            int page = 1,
            bool? redirectToCheck = false)
        {
            var certificate = await _apiClient.GetCertificate(certificateId);
            var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);

            var vm = new DuplicateRequestViewModel
            {
                CertificateId = certificateId,
                IsConfirmed = false,
                NextBatchDate = "Fake Date",
                SearchString = searchString,
                CertificateReference = certificate.CertificateReference,
                StdCode = stdCode,
                Uln = uln,
                FullName = certificateData.FullName,
                Status = certificate.Status,
                BackToCheckPage = redirectToCheck.Value,
                Page = page
            };

            return View(vm);
        }

        [HttpPost(Name = "Index")]
        public async Task<IActionResult> Index(DuplicateRequestViewModel duplicateRequestViewModel)
        {
            var username = _contextAccessor.HttpContext.User
                .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;
            var certificate = await _apiClient.PostReprintRequest(new StaffCertificateDuplicateRequest
            {
                Id = duplicateRequestViewModel.CertificateId,
                Username = username
            });
            var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);

            var nextScheduledRun = await _apiClient.GetNextScheduledRun((int) ScheduleType.PrintRun);
            var vm = new DuplicateRequestViewModel
            {
                CertificateId = certificate.Id,
                IsConfirmed = true,
                NextBatchDate = nextScheduledRun?.RunTime.ToString("dd/MM/yyyy"),
                CertificateReference = certificate.CertificateReference,
                SearchString = duplicateRequestViewModel.SearchString,
                StdCode = duplicateRequestViewModel.StdCode,
                Uln = duplicateRequestViewModel.Uln,
                Status = certificate.Status,
                FullName = certificateData.FullName,
                Page = duplicateRequestViewModel.Page
            };

            return View(vm);
        }
    }
}