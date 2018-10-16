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
    [Authorize(Policy = Startup.Policies.OperationsTeamOnly)]
    public class CommentController : Controller
    {
        private readonly ApiClient _apiClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public CommentController(ApiClient apiClient,
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
                Page = page,
                BackToCheckPage = redirectToCheck.Value
            };

            return View(vm);
        }            
    }
}