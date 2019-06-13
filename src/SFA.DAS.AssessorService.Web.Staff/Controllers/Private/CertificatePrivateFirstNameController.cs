using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Private;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers.Private
{
    [Authorize]
    [Route("certificate/firstname")]
    public class CertificatePrivateFirstNameController : CertificateBaseController
    {
        public CertificatePrivateFirstNameController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient) : base(logger, contextAccessor, apiClient)
        { }

        [HttpGet]
        public async Task<IActionResult> FirstName(Guid certificateId, bool fromApproval)
        {
            var viewModel = await LoadViewModel<CertificateFirstNameViewModel>(certificateId, "~/Views/CertificateAmend/FirstName.cshtml");
            if (viewModel is ViewResult viewResult && viewResult.Model is CertificateFirstNameViewModel certificateFirstNameViewModel)
                certificateFirstNameViewModel.FromApproval = fromApproval;

            return viewModel;
        }

        [HttpPost(Name = "FirstName")]
        public async Task<IActionResult> FirstName(CertificateFirstNameViewModel vm)
        {
            var actionResult = await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmend/FirstName.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend", new { certificateId = vm.Id , fromapproval = vm.FromApproval }), action: CertificateActions.FirstName);

            return actionResult;
        }
    }
}