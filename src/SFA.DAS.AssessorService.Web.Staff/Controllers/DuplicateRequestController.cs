using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    public class DuplicateRequestController : Controller
    {
        private readonly ApiClient _apiClient;

        public DuplicateRequestController(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid certificateId)
        {
            var certificate = await _apiClient.GetCertificate(certificateId);

            var vm = new DuplicateRequestViewModel();
            vm.Certificate = certificate;
            vm.IsConfirmed = false;
            vm.NextBatchDate = "Fake Date";
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