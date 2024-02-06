using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Validators;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Route("certificate/givennames")]
    public class CertificateGivenNamesController : CertificateBaseController
    {
        private readonly CertificateGivenNamesViewModelValidator _certificateNameChangeValidator;

        public CertificateGivenNamesController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor, ICertificateApiClient certificateApiClient, ISessionService sessionService, CertificateGivenNamesViewModelValidator certificateNameChangeValidator) : base(logger, contextAccessor, certificateApiClient, sessionService)
        {
            _certificateNameChangeValidator = certificateNameChangeValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GivenNames()
        {
            return await LoadViewModel<CertificateGivenNamesViewModel>("~/Views/Certificate/GivenNames.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> GivenNames(CertificateGivenNamesViewModel viewModel)
        {
            var validationResult = await _certificateNameChangeValidator.Validate(viewModel);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View("~/Views/Certificate/GivenNames.cshtml", viewModel);
            }

            return await SaveViewModel(viewModel,
                returnToIfModelNotValid: "~/Views/Certificate/GivenNames.cshtml",
                nextAction: RedirectToAction("Check", "CertificateCheck"), action: CertificateActions.GivenNames);
        }
    }
}
