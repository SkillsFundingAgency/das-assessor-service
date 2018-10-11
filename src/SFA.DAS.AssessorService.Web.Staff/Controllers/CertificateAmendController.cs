using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;

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
        public async Task<IActionResult> Check(Guid certificateid,           
            string searchString,
            int page)
        {
            var viewModel =
                await LoadViewModel<CertificateCheckViewModel>(certificateid, "~/Views/CertificateAmend/Check.cshtml");
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
            return RedirectToAction("Index", "DuplicateRequest",
                new
                {
                    certificateid = vm.Id, redirectToCheck = vm.RedirectToCheck,
                    Uln = vm.Uln,
                    StdCode = vm.StandardCode,
                    Page = vm.Page,
                    SearchString = vm.SearchString
                });
        }
    }
}