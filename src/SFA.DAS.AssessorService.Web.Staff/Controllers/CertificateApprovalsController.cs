using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Private;
using AutoMapper;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    [Authorize(Policy = Startup.Policies.OperationsTeamOnly)]
    public class CertificateApprovalsController : CertificateBaseController
    {
        public CertificateApprovalsController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient)
            : base(logger, contextAccessor, apiClient)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Approvals()
        {
            var certificates = await ApiClient.GetCertificatesToBeApproved();
            var certificatesToBeApproved = new CertificateApprovalViewModel
            {
                CertificateDetailApprovalViewModels = Mapper.Map<List<CertificateDetailApprovalViewModel>>(certificates)
            };

            return View(certificatesToBeApproved);
        }

        [HttpPost(Name = "Approvals")]
        public async Task<IActionResult> Approvals(
            CertificatePostApprovalViewModel certificateApprovalViewModel)
        {
            certificateApprovalViewModel.UserName = ContextAccessor.HttpContext.User
                .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            ApiClient.ApproveCertificates(certificateApprovalViewModel);

            return RedirectToAction("Index", "Dashboard");
        }
    }
}