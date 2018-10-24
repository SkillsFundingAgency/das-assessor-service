using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.Validators;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers.Private
{
    [Authorize]
    [Route("certificate/learningstartdate")]
    public class CertificatePrivateLearnerStartDateController : CertificateBaseController
    {
        private readonly CertificateLearnerStartDateViewModelValidator _validator;

        public CertificatePrivateLearnerStartDateController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient,
            CertificateLearnerStartDateViewModelValidator certificateLearnerStartDateViewModelValidator) : base(logger,
            contextAccessor, apiClient)
        {
            _validator = certificateLearnerStartDateViewModelValidator;
        }
       
        [HttpGet]
        public async Task<IActionResult> LearnerStartDate(Guid certificateid)
        {
            return await LoadViewModel<CertificateLearnerStartDateViewModel>(certificateid, "~/Views/CertificateAmend/LearnerStartDate.cshtml");
        }

        [HttpPost(Name = "LearnerStartDate")]
        public async Task<IActionResult> LearnerStartDate(CertificateLearnerStartDateViewModel vm)
        {
            var result = _validator.Validate(vm);

            if (!result.IsValid && result.Errors.Any(e => e.Severity == Severity.Warning))
            {
                vm.WarningShown = "true";
                return View("~/Views/CertificateAmend/LearnerStartDate.cshtml", vm);
            }

            var actionResult = await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmend/LearnerStartDate.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend", new { certificateid = vm.Id }), action: CertificateActions.LearnerStartDate);

            return actionResult;
        }
    }
}