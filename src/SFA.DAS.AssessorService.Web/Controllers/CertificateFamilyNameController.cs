using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using System.Threading.Tasks;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Route("certificate/familyname")]
    public class CertificateFamilyNameController : CertificateBaseController
    {
        private readonly IValidator<CertificateNamesViewModel> _certificateNameChangeValidator;
        public CertificateFamilyNameController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor, ICertificateApiClient certificateApiClient, ISessionService sessionService, IValidator<CertificateNamesViewModel> certificateNameChangeValidator) : base(logger, contextAccessor, certificateApiClient, sessionService)
        {
            _certificateNameChangeValidator = certificateNameChangeValidator;
        }

        [HttpGet]
        public async Task<IActionResult> FamilyName(bool? redirectToCheck = true)
        {
            return await LoadViewModel<CertificateNamesViewModel>("~/Views/Certificate/FamilyName.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> FamilyName(CertificateNamesViewModel viewModel)
        {
            var validationResult = _certificateNameChangeValidator.Validate(viewModel);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View("/Views/Certificate/FamilyName.cshtml", viewModel);
            }

            viewModel.FamilyName = viewModel.InputFamilyName;

            return await SaveViewModel(viewModel,
                returnToIfModelNotValid: "~/Views/Certificate/FamilyName.cshtml",
                nextAction: RedirectToAction("Check", "CertificateCheck"), action: CertificateActions.LastName);
        }

        protected async Task<IActionResult> LoadCertificateFamilyNameViewModel<T>(string view) where T : CertificateBaseViewModel, new()
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

            viewModel.InputFamilyName = viewModel.FamilyName;

            return View(view, viewModel);
        }
    }
}
