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
        public async Task<IActionResult> Check(Guid certificateId, string searchString, int page)
        {
            var viewModel = await LoadViewModel<CertificateCheckViewModel>(certificateId, "~/Views/CertificateAmend/Check.cshtml");
            var viewResult = (viewModel as ViewResult);
            var certificateCheckViewModel = viewResult.Model as CertificateCheckViewModel;

            certificateCheckViewModel.Page = page;
            certificateCheckViewModel.SearchString = searchString;


            var options = await ApiClient.GetOptions(certificateCheckViewModel.StandardCode);
            TempData["HideOption"] = !options.Any();

            return viewModel;
        }

        [HttpPost(Name = "Check")]
        public async Task<IActionResult> ConfirmAndSubmit(CertificateCheckViewModel vm)
        {
            if (vm.Status ==
                CertificateStatus.Submitted ||
                vm.Status == CertificateStatus.Printed ||
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