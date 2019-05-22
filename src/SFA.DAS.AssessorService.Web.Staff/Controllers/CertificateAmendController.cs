using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Private;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    [Authorize]
    public class CertificateAmendController : CertificateBaseController
    {
        public CertificateAmendController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient) : base(logger, contextAccessor, apiClient)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Check(Guid certificateId, string searchString, int page, bool fromApproval)
        {
            var viewModel = await LoadViewModel<CertificateCheckViewModel>(certificateId, "~/Views/CertificateAmend/Check.cshtml");
            var viewResult = (viewModel as ViewResult);
            var certificateCheckViewModel = viewResult.Model as CertificateCheckViewModel;

            certificateCheckViewModel.Page = page;
            certificateCheckViewModel.SearchString = searchString;
            certificateCheckViewModel.FromApproval = fromApproval;

            var options = await ApiClient.GetOptions(certificateCheckViewModel.StandardCode);
            TempData["HideOption"] = !options.Any();

            return viewModel;
        }


        [HttpPost(Name = "Check")]
        public async Task<IActionResult> ConfirmAndSubmit(CertificateCheckViewModel vm)
        {
            if (vm.Status == CertificateStatus.Printed ||
                vm.Status == CertificateStatus.Reprint)
            {
                return RedirectToAction("Index", "DuplicateRequest",
                    new
                    {
                        certificateId = vm.Id, redirectToCheck = vm.RedirectToCheck,
                        Uln = vm.Uln,
                        StdCode = vm.StandardCode,                     
                        Page = vm.Page,
                        SearchString = vm.SearchString
                    });
            }

            if (vm.Status == CertificateStatus.Draft &
                vm.PrivatelyFundedStatus == CertificateStatus.Rejected & vm.FromApproval)
            {
                var certificate = await ApiClient.GetCertificate(vm.Id);
                var approvalResults = new ApprovalResult[1];
                approvalResults[0]= new ApprovalResult
                {
                    IsApproved = CertificateStatus.Submitted,
                    CertificateReference = certificate.CertificateReference,
                    PrivatelyFundedStatus = CertificateStatus.Approved
                };
                
                await ApiClient.ApproveCertificates(new CertificatePostApprovalViewModel
                {
                    UserName = ContextAccessor.HttpContext.User
                    .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value,
                    ApprovalResults = approvalResults
                });
                return RedirectToAction("Approved", "CertificateApprovals");
            }
            else
            {
                return RedirectToAction("Index", "Comment",
                    new
                    {
                        certificateId = vm.Id, redirectToCheck = vm.RedirectToCheck,
                        Uln = vm.Uln,
                        StdCode = vm.StandardCode,
                        Page = vm.Page,                       
                        SearchString = vm.SearchString
                    });
            }
        }
    }
}