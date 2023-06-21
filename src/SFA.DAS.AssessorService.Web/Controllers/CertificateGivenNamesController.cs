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
        public async Task<IActionResult> GivenNames(bool? redirectToCheck = true)
        {
            return await LoadCertificateGivenNamesViewModel<CertificateNamesViewModel>("~/Views/Certificate/GivenNames.cshtml");
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

        protected async Task<IActionResult> LoadCertificateGivenNamesViewModel<T>(string view) where T : CertificateBaseViewModel, new()
        {
            var username = GetUsernameFromClaim();

            Logger.LogInformation($"Load View Model for {typeof(T).Name} for {username}");

            var viewModel = new CertificateNamesViewModel();

            CheckAndSetRedirectToCheck(viewModel);

            if (!TryGetCertificateSession(typeof(T).Name, username, out var certSession))
            {
                return RedirectToAction("Index", "Search");
            }

            var certificate = await CertificateApiClient.GetCertificate(certSession.CertificateId);

            Logger.LogInformation($"Got Certificate for {typeof(T).Name} requested by {username} with Id {certificate.Id}");

            viewModel.FromCertificate(certificate);

            Logger.LogInformation($"Got View Model of type {typeof(T).Name} requested by {username}");

            viewModel.InputGivenNames = viewModel.GivenNames;

            return View(view, viewModel);
        }
    }
}
