using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using SFA.DAS.AssessorService.Web.ViewModels.Shared;
using static SFA.DAS.AssessorService.Web.ViewModels.Certificate.CertificateVersionViewModel;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [Route("certificate/version")]
    public class CertificateVersionController : CertificateBaseController
    {
        private readonly IStandardVersionClient _standardVersionClient;
        private readonly IStandardServiceClient _standardServiceClient;
        public CertificateVersionController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient, IStandardVersionClient standardVersionClient, IStandardServiceClient standardServiceClient, ISessionService sessionService)
            : base(logger, contextAccessor, certificateApiClient, sessionService)
        {
            _standardVersionClient = standardVersionClient;
            _standardServiceClient = standardServiceClient;
        }

        [HttpGet]
        public async Task<IActionResult> Version(bool? redirectToCheck = false)
        {
            var sessionString = SessionService.Get(nameof(CertificateSession));
            if (sessionString == null)
            {
                return RedirectToAction("Index", "Search");
            }

            return await LoadViewModel("~/Views/Certificate/Version.cshtml");
        }

        private async Task<IActionResult> LoadViewModel(string view)
        {
            var username = GetUsernameFromClaim();

            Logger.LogInformation($"Load View Model for CertificateVersionViewModel for {username}");

            var viewModel = new CertificateVersionViewModel();

            CheckAndSetRedirectToCheck(viewModel);

            if (!TryGetCertificateSession("CertificateVersionViewModel", username, out CertificateSession certSession))
            {
                return RedirectToAction("Index", "Search");
            }

            var certificate = await CertificateApiClient.GetCertificate(certSession.CertificateId);

            if (certSession.Versions == null || certSession.Versions.Count() == 0)
            {
                return RedirectToAction("Index", "Search");
            }

            if (certSession.Versions.Count() == 1)
            {
                // Only 1 version no need for a selection
                var singularStandard = certSession.Versions.First();
                var options = await _standardServiceClient.GetStandardOptions(singularStandard.StandardUId);
                if (options != null & options.HasOptions())
                {
                    certSession.StandardUId = singularStandard.StandardUId;
                    certSession.Options = options.CourseOption.ToList();
                    SessionService.Set(nameof(CertificateSession), certSession);
                    return RedirectToAction("Option", "CertificateOption");
                }

                return RedirectToAction("Declare", "CertificateDeclaration");
            }

            Logger.LogInformation($"Got Certificate for CertificateVersionViewModel requested by {username} with Id {certificate.Id}");

            viewModel.FromCertificate(certificate, certSession.Versions);

            Logger.LogInformation($"Got View Model of type CertificateVersionViewModel requested by {username}");

            return View(view, viewModel);
        }

        [HttpPost(Name = "Version")]
        public async Task<IActionResult> Version(CertificateVersionViewModel vm)
        {
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/Certificate/Version.cshtml",
                nextAction: RedirectToAction("Declare", "CertificateDeclaration"), action: CertificateActions.Option);
        }

        private async Task<IActionResult> SaveViewModel(CertificateVersionViewModel vm, string returnToIfModelNotValid, RedirectToActionResult nextAction, string action)
        {
            var username = GetUsernameFromClaim();
            var epaoid = GetEpaOrgIdFromClaim();
            SessionService.TryGet<bool>("redirecttocheck", out var redirectToCheck);

            Logger.LogInformation($"Save View Model for CertificateVersionViewModel for {username} with values: {GetModelValues(vm)}");

            var certificate = await CertificateApiClient.GetCertificate(vm.Id);
            var certData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);

            if (!TryGetCertificateSession("CertificateVersionViewModel", username, out var certSession))
            {
                return RedirectToAction("Index", "Search");
            }

            if (!ModelState.IsValid)
            {
                Logger.LogInformation($"Model State not valid for CertificateVersionViewModel requested by {username} with Id {certificate.Id}. Errors: {ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)}");
                return View(returnToIfModelNotValid, vm);
            }

            var standardVersion = await _standardVersionClient.GetStandardVersionByStandardUId(vm.StandardUId);
            var approvedStandardVersions = await _standardVersionClient.GetEpaoRegisteredStandardVersions(epaoid, certSession.StandardCode);
            var options = await _standardServiceClient.GetStandardOptions(vm.StandardUId);

            if (!approvedStandardVersions.Any(v => v.StandardUId == vm.StandardUId))
            {
                // Epao not approved for this version
                ModelState.AddModelError("StandardUId", $"Your organisation is not approved to assess version {standardVersion.Version} of {standardVersion.Title}");
                vm.Versions = certSession.Versions;
                vm.BackToCheckPage = redirectToCheck;
                return View(returnToIfModelNotValid, vm);
            }
                        
            var versionChanged = certificate.StandardUId != vm.StandardUId;
            // Edge case to cater to back buttons where user can change version without setting an option
            var optionNotSet = string.IsNullOrEmpty(certData.CourseOption) && options != null && options.HasOptions();

            if (!versionChanged && !optionNotSet && redirectToCheck)
            {
                // if version hasn't changed, and option set if required, don't need to update options.
                return new RedirectToActionResult("Check", "CertificateCheck", null);
            }

            certSession.StandardUId = vm.StandardUId;
                        
            // To pass in to inherited method.
            vm.SelectedStandardVersion = standardVersion;
            vm.SelectedStandardOptions = options;
            var updatedCertificate = vm.GetCertificateFromViewModel(certificate, certData);
            await CertificateApiClient.UpdateCertificate(new UpdateCertificateRequest(updatedCertificate) { Username = username, Action = action });
            
            Logger.LogInformation($"Certificate for CertificateVersionViewModel requested by {username} with Id {certificate.Id} updated.");

            if (options != null && options.HasOptions())
            {
                certSession.Options = options.CourseOption.ToList();
                SessionService.Set(nameof(CertificateSession), certSession);

                if (options.OnlyOneOption())
                {
                    if (redirectToCheck)
                    {
                        return new RedirectToActionResult("Check", "CertificateCheck", null);
                    }

                    return new RedirectToActionResult("Declare", "CertificateDeclaration", null);
                }

                object routeValues = null;
                if (redirectToCheck)
                {
                    routeValues = new { redirecttocheck = true };
                }

                SessionService.Set("redirectedfromversion", true);
                return new RedirectToActionResult("Option", "CertificateOption", routeValues);
            }
            else
            {
                certSession.Options = null;
                SessionService.Set(nameof(CertificateSession), certSession);
            }

            if (redirectToCheck)
            {
                Logger.LogInformation($"Certificate for CertificateVersionViewModel requested by {username} with Id {certificate.Id} redirecting back to Certificate Check.");
                return new RedirectToActionResult("Check", "CertificateCheck", null);
            }

            Logger.LogInformation($"Certificate for CertificateVersionViewModel requested by {username} with Id {certificate.Id} redirecting to {nextAction.ControllerName} {nextAction.ActionName}");
            return nextAction;
        }

        private string GetModelValues<T>(T viewModel)
        {
            var properties = typeof(T).GetProperties().ToList();

            return properties.Aggregate("", (current, prop) => current + $"{prop.Name}: {prop.GetValue(viewModel)}, ");
        }
    }
}