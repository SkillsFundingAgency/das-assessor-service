using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;
using System;
using System.Threading.Tasks;
using CertificateActions = SFA.DAS.AssessorService.Domain.Consts.CertificateActions;
using CertificateStatus = SFA.DAS.AssessorService.Domain.Consts.CertificateStatus;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    public class CertificateAmendController : CertificateBaseController
    {
        public CertificateAmendController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient) : base(logger, contextAccessor, apiClient)
        {

        }

        [HttpGet]
        public async Task<IActionResult> Check(Guid certificateId)
        {
            return await LoadViewModel<CertificateCheckViewModel>(certificateId, "~/Views/CertificateAmend/Check.cshtml");
        }     

        [HttpPost]
        public async Task<IActionResult> ConfirmAndSubmit(CertificateCheckViewModel vm)
        {
            var certificate = await ApiClient.GetCertificate(vm.Id);

            if(certificate.Status == CertificateStatus.Printed || certificate.Status == CertificateStatus.Reprint)
            {
                return RedirectToAction("Index", "DuplicateRequest", new { certificateId = certificate.Id });
            }
            else if(certificate.Status == CertificateStatus.Draft || certificate.Status == CertificateStatus.Submitted)
            {
                return RedirectToAction("Select", "Search", new { stdcode = certificate.StandardCode, uln = certificate.Uln, searchString = certificate.Uln });
            }

            return RedirectToAction("Index", "Search");
        }
    }
}