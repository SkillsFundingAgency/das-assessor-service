using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Validators;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private;

namespace SFA.DAS.AssessorService.Web.Controllers.Private
{
    [Authorize]
    [Route("certificate/learningstartdate")]
    public class CertificatePrivateLearnerStartDateController : CertificateBaseController
    {
        private readonly CertificateLearnerStartDateViewModelValidator _validator;

        public CertificatePrivateLearnerStartDateController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient, CertificateLearnerStartDateViewModelValidator validator, ISessionService sessionService) : base(logger, contextAccessor, certificateApiClient, sessionService)
        {
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> LearnerStartDate(bool? redirectToCheck = false,
            bool? redirecttosearch = false)
        {
            return await LoadViewModel<CertificateLearnerStartDateViewModel>("~/Views/Certificate/LearnerStartDate.cshtml");
        }

        [HttpPost(Name = "LearnerStartDate")]
        public async Task<IActionResult> LearnerStartDate(CertificateLearnerStartDateViewModel vm)
        {
            var result = _validator.Validate(vm);

            if (!result.IsValid && result.Errors.Any(e => e.Severity == Severity.Warning))
            {
                vm.WarningShown = "true";
                return View("~/Views/Certificate/LearnerStartDate.cshtml", vm);
            }

            var actionResult = await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/Certificate/LearnerStartDate.cshtml",
                nextAction: RedirectToAction("Ukprn", "CertificatePrivateProviderUkprn"), action: CertificateActions.LearnerStartDate);

            return actionResult;
        }
    }
}