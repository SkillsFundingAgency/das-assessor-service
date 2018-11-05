using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.Validators;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    public class CertificateDateController : CertificateBaseController
    {
        private readonly CertificateDateViewModelValidator _validator;

        public CertificateDateController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient,
            CertificateDateViewModelValidator validator) : base(logger, contextAccessor, apiClient)
        {
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> Date(Guid certificateId)
        {
            return await LoadViewModel<CertificateDateViewModel>(certificateId, "~/Views/CertificateAmend/Date.cshtml");
        }

        [HttpPost(Name = "Date")]
        public async Task<IActionResult> Date(CertificateDateViewModel vm)
        {
            var result = _validator.Validate(vm);

            var actionResult = await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmend/Date.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend", new { certificateId = vm.Id }), action: CertificateActions.Date);

            return actionResult;
        }
    }
}