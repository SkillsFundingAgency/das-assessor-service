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
            var sessionString = SessionService.Get("CertificateSession");
            if (sessionString == null)
            {
                return RedirectToAction("Index", "Search");
            }

            return await LoadViewModel("~/Views/Certificate/Version.cshtml");
        }

        private async Task<IActionResult> LoadViewModel(string view)
        {
            var username = ContextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            Logger.LogInformation($"Load View Model for CertificateVersionViewModel for {username}");

            var viewModel = new CertificateVersionViewModel();

            var query = ContextAccessor.HttpContext.Request.Query;
            if (query.ContainsKey("redirecttocheck") && bool.Parse(query["redirecttocheck"]))
            {
                Logger.LogInformation($"RedirectToCheck for CertificateVersionViewModel is true");
                SessionService.Set("redirecttocheck", "true");
                viewModel.BackToCheckPage = true;
            }
            else
            {
                SessionService.Remove("redirecttocheck");
            }

            var sessionString = SessionService.Get("CertificateSession");
            if (sessionString == null)
            {
                Logger.LogInformation($"Session for CertificateVersionViewModel requested by {username} has been lost. Redirecting to Search Index");
                return RedirectToAction("Index", "Search");
            }

            var certSession = JsonConvert.DeserializeObject<CertificateSession>(sessionString);
            var certificate = await CertificateApiClient.GetCertificate(certSession.CertificateId);

            var versions = await _standardVersionClient.GetStandardVersionsByLarsCode(certSession.StandardCode);

            if (versions.Count() == 1)
            {
                // Only 1 version no need for a selection
                var singularStandard = versions.First();
                var options = await _standardServiceClient.GetStandardOptions(singularStandard.StandardUId);
                if (options != null & options.HasOptions())
                {
                    certSession.StandardUId = singularStandard.StandardUId;
                    certSession.Options = options.CourseOption.ToList();
                    SessionService.Set("CertificateSession", certSession);
                    return RedirectToAction("Option", "CertificateOption");
                }

                return RedirectToAction("Declare", "CertificateDeclaration");
            }

            Logger.LogInformation($"Got Certificate for CertificateVersionViewModel requested by {username} with Id {certificate.Id}");

            viewModel.FromCertificate(certificate, versions.Select(v => (StandardVersion)v));

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
            var username = ContextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;
            var epaoid = ContextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;

            Logger.LogInformation($"Save View Model for CertificateVersionViewModel for {username} with values: {GetModelValues(vm)}");

            var certificate = await CertificateApiClient.GetCertificate(vm.Id);

            var sessionString = SessionService.Get("CertificateSession");
            if (sessionString == null)
            {
                Logger.LogInformation($"Session for CertificateVersionViewModel requested by {username} has been lost. Redirecting to Search Index");
                return RedirectToAction("Index", "Search");
            }
            var certSession = JsonConvert.DeserializeObject<CertificateSession>(sessionString);

            if (!ModelState.IsValid)
            {
                Logger.LogInformation($"Model State not valid for CertificateVersionViewModel requested by {username} with Id {certificate.Id}. Errors: {ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)}");
                return View(returnToIfModelNotValid, vm);
            }

            // collected here as it's needed later and GetCertificateFromViewModel Updates Cert with View Model
            var standardVersionChanged = vm.StandardUId != certificate.StandardUId;
            var standardVersion = await _standardVersionClient.GetStandardVersionByStandardUId(vm.StandardUId);

            // Retrieve what versions the EPAO is able to assess
            var approvedStandardVersions = await _standardVersionClient.GetEpaoRegisteredStandardVersions(epaoid, certSession.StandardCode);
            if (!approvedStandardVersions.Any(v => v.StandardUId == vm.StandardUId))
            {
                // Epao not approved for this version
                ModelState.AddModelError("StandardUId", $"Your organisation is not approved to assess version {standardVersion.Version} of {standardVersion.Title}");
                var versions = await _standardVersionClient.GetStandardVersionsByLarsCode(certSession.StandardCode);
                vm.Versions = versions.Select(v => (StandardVersion)v);
                return View(returnToIfModelNotValid, vm);
            }

            var updatedCertificate = vm.GetCertificateFromViewModel(certificate, standardVersionChanged, standardVersion);

            await CertificateApiClient.UpdateCertificate(new UpdateCertificateRequest(updatedCertificate) { Username = username, Action = action });

            Logger.LogInformation($"Certificate for CertificateVersionViewModel requested by {username} with Id {certificate.Id} updated.");

            if (standardVersionChanged)
            {
                certSession.Options = null;
                certSession.StandardUId = vm.StandardUId;
                SessionService.Set("CertificateSession", certSession);

                var options = await _standardServiceClient.GetStandardOptions(vm.StandardUId);

                if (options != null && options.HasOptions())
                {
                    certSession.Options = options.CourseOption.ToList();
                    SessionService.Set("CertificateSession", certSession);
                    object routeValues = null;
                    if (SessionService.Exists("redirecttocheck") && bool.Parse(SessionService.Get("redirecttocheck")))
                    {
                        routeValues = new { redirecttocheck = true };
                    }

                    return new RedirectToActionResult("Option", "CertificateOption", routeValues);
                }
            }

            if (SessionService.Exists("redirecttocheck") && bool.Parse(SessionService.Get("redirecttocheck")))
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