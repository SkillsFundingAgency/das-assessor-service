using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Route("certificate/givennames")]
    public class CertificateGivenNamesController : CertificateBaseController
    {
        private readonly IValidator<CertificateNamesViewModel> _certificateNameChangeValidator;

        public CertificateGivenNamesController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor, ICertificateApiClient certificateApiClient, ISessionService sessionService, IValidator<CertificateNamesViewModel> certificateNameChangeValidator) : base(logger, contextAccessor, certificateApiClient, sessionService)
        {
            _certificateNameChangeValidator = certificateNameChangeValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GivenNames()
        {
            return await LoadViewModel<CertificateNamesViewModel>("~/Views/Certificate/GivenNames.cshtml", false);
        }

        [HttpPost]
        public async Task<IActionResult> GivenNames(CertificateNamesViewModel viewModel)
        {
            var validationResult = _certificateNameChangeValidator.Validate(viewModel);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View("~/Views/Certificate/GivenNames.cshtml", viewModel);
            }

            viewModel.GivenNames = viewModel.InputGivenNames;

            return await SaveViewModel(viewModel,
                returnToIfModelNotValid: "~/Views/Certificate/GivenNames.cshtml",
                nextAction: RedirectToAction("Check", "CertificateCheck"), action: CertificateActions.FirstName);
        }
    }
}
