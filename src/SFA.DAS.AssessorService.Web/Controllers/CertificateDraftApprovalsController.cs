using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    [Authorize]
    public class CertificateDraftApprovalsController : Controller
    {
        private readonly ILogger<CertificateDraftApprovalsController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICertificateApiClient _certificateApiClient;

        public CertificateDraftApprovalsController(ILogger<CertificateDraftApprovalsController> logger,
            IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _certificateApiClient = certificateApiClient;
        }

        [HttpGet]
        public async Task<IActionResult> CertificateDraftApprovals()
        {
            var certificates = await _certificateApiClient.GetDraftCertificatesInApprovalState();
            var certificatesToBeApproved = new CertificateApprovalViewModel
            {
                DraftCertificates = Mapper.Map<List<CertificateDetailApprovalViewModel>>(certificates)
            };

            return View("DraftApprovals", certificatesToBeApproved);
        }
    }
}