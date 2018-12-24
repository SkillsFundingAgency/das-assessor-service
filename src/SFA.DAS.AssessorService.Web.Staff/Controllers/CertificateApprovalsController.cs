using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Private;
using AutoMapper;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{  
    [Authorize]
    public class CertificateApprovalsController : CertificateBaseController
    {
        public CertificateApprovalsController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient)
            : base(logger, contextAccessor, apiClient)
        {
        }
        

        [HttpGet]
        public async Task<IActionResult> New()
        {
            var certificates = await ApiClient.GetCertificatesToBeApproved();
            var certificatesToBeApproved = new CertificateApprovalViewModel
            {
                ToBeApprovedCertificates = Mapper.Map<List<CertificateDetailApprovalViewModel>>(certificates
                    .Where(q => q.Status == CertificateStatus.ToBeApproved && q.PrivatelyFundedStatus == null))
            };

            return View(certificatesToBeApproved);
        }

        [HttpGet]
        public async Task<IActionResult> Sent()
        {
            var certificates = await ApiClient.GetCertificatesToBeApproved();
            var certificatesSentForApproval = new CertificateApprovalViewModel
            {
                SentForApprovalCertificates = Mapper.Map<List<CertificateDetailApprovalViewModel>>(certificates
                    .Where(q => q.Status == CertificateStatus.ToBeApproved && 
                                q.PrivatelyFundedStatus == CertificateStatus.SentForApproval)),
               
            };

            return View(certificatesSentForApproval);
        }

        [HttpGet]
        public async Task<IActionResult> Approved()
        {
            var certificates = await ApiClient.GetCertificatesToBeApproved();
            var certificatesApproved = new CertificateApprovalViewModel
            {

                ApprovedCertificates = Mapper.Map<List<CertificateDetailApprovalViewModel>>(certificates
                    .Where(q => q.Status == CertificateStatus.Submitted && q.PrivatelyFundedStatus == CertificateStatus.Approved))
            };

            return View(certificatesApproved);
        }

        [HttpGet]
        public async Task<IActionResult> Rejected()
        {
            var certificates = await ApiClient.GetCertificatesToBeApproved();
            var certificatesThatAreRejected = new CertificateApprovalViewModel
            {

                RejectedCertificates = Mapper.Map<List<CertificateDetailApprovalViewModel>>(certificates
                    .Where(q => q.Status == CertificateStatus.Draft && q.PrivatelyFundedStatus == CertificateStatus.Rejected))
            };

            return View(certificatesThatAreRejected);
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