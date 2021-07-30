using FluentValidation;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    [Route("certificate/check")]
    public class CertificateCheckController : CertificateBaseController
    {
        private readonly IValidator<CertificateCheckViewModel> _validator;

        public CertificateCheckController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient, IValidator<CertificateCheckViewModel> validator, ISessionService sessionService) : base(logger, contextAccessor, certificateApiClient, sessionService)
        {
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> Check()
        {
            var sessionString = SessionService.Get(nameof(CertificateSession));
            if (sessionString == null)
            {
                return RedirectToAction("Index", "Search");
            }
           
            return await LoadViewModel("~/Views/Certificate/Check.cshtml");
        }

        [HttpPost(Name = "Check")]
        public async Task<IActionResult> Check(CertificateCheckViewModel vm)
        {
            // This is the final step in the process so get the latest version of the certificate!
            if (vm is null) vm = new CertificateCheckViewModel();
            vm.FromCertificate(await CertificateApiClient.GetCertificate(vm.Id));

            var result = _validator.Validate(vm);

            if (!result.IsValid)
            {
                return await Check();
            }

            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/Certificate/Check.cshtml",
                nextAction: RedirectToAction("Confirm", "CertificateConfirmation"), action: CertificateActions.Submit);
        }

        private async Task<IActionResult> LoadViewModel(string view)
        {
            var username = GetUsernameFromClaim();

            Logger.LogInformation($"Load View Model for CertificateCheckViewModel for {username}");

            var viewModel = new CertificateCheckViewModel();

            CheckAndSetRedirectToCheck(viewModel);

            if (!TryGetCertificateSession("CertificateCheckViewModel", username, out var certSession))
            {
                return RedirectToAction("Index", "Search");
            }

            var certificate = await CertificateApiClient.GetCertificate(certSession.CertificateId);

            Logger.LogInformation($"Got Certificate for CertificateCheckViewModel requested by {username} with Id {certificate.Id}");

            viewModel.FromCertificate(certificate);
            viewModel.SetStandardHasVersionsAndOptions(certSession);

            Logger.LogInformation($"Got View Model of type CertificateCheckViewModel requested by {username}");

            return View(view, viewModel);
        }
    }
}