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

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [Route("certificate/option")]
    public class CertificateOptionController : CertificateBaseController
    {
        public CertificateOptionController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor, ICertificateApiClient certificateApiClient, ISessionService sessionService)
            :base(logger, contextAccessor, certificateApiClient, sessionService)
        {
            
        }

        [HttpGet]
        public async Task<IActionResult> Option(bool? redirectToCheck = false)
        {
            var sessionString = SessionService.Get(nameof(CertificateSession));
            if (sessionString == null)
            {
                return RedirectToAction("Index", "Search");
            }

            return await LoadViewModel("~/Views/Certificate/Option.cshtml");
        }

        private async Task<IActionResult> LoadViewModel(string view)
        {
            var username = ContextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            Logger.LogInformation($"Load View Model for CertificateOptionViewModel for {username}");

            var viewModel = new CertificateOptionViewModel();

            var query = ContextAccessor.HttpContext.Request.Query;
            if (query.ContainsKey("redirecttocheck") && bool.Parse(query["redirecttocheck"]))
            {
                Logger.LogInformation($"RedirectToCheck for CertificateOptionViewModel is true");
                SessionService.Set("redirecttocheck", "true");
                viewModel.BackToCheckPage = true;
            }
            else
                SessionService.Remove("redirecttocheck");
                
            var sessionString = SessionService.Get(nameof(CertificateSession));
            if (sessionString == null)
            {
                Logger.LogInformation($"Session for CertificateOptionViewModel requested by {username} has been lost. Redirecting to Search Index");
                return RedirectToAction("Index", "Search");
            }
            var certSession = JsonConvert.DeserializeObject<CertificateSession>(sessionString);

            var certificate = await CertificateApiClient.GetCertificate(certSession.CertificateId);

            if (certSession.Options == null || !certSession.Options.Any())
            {
                return RedirectToAction("Index", "Search");
            }

            Logger.LogInformation($"Got Certificate for CertificateOptionViewModel requested by {username} with Id {certificate.Id}");

            viewModel.FromCertificate(certificate, certSession.Options);

            Logger.LogInformation($"Got View Model of type CertificateOptionViewModel requested by {username}");

            return View(view, viewModel);
        }

        [HttpPost(Name = "Option")]
        public async Task<IActionResult> Option(CertificateOptionViewModel vm)
        {
         
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/Certificate/Option.cshtml",
                nextAction: RedirectToAction("Declare", "CertificateDeclaration"), action: CertificateActions.Option);
        }

         private async Task<IActionResult> SaveViewModel(CertificateOptionViewModel vm, string returnToIfModelNotValid, RedirectToActionResult nextAction, string action)
        {
            var username = ContextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;
            
            Logger.LogInformation($"Save View Model for CertificateOptionViewModel for {username} with values: {GetModelValues(vm)}");
            
            var certificate = await CertificateApiClient.GetCertificate(vm.Id);
            var certData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
            SessionService.Remove("redirectedfromversion");

            var sessionString = SessionService.Get(nameof(CertificateSession));
            if (sessionString == null)
            {
                Logger.LogInformation($"Session for CertificateOptionViewModel requested by {username} has been lost. Redirecting to Search Index");
                return RedirectToAction("Index", "Search");
            }
            var certSession = JsonConvert.DeserializeObject<CertificateSession>(sessionString);

            if (!ModelState.IsValid)
            {
                vm.Options = certSession.Options;
                Logger.LogInformation($"Model State not valid for CertificateOptionViewModel requested by {username} with Id {certificate.Id}. Errors: {ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)}");
                return View(returnToIfModelNotValid, vm);
            }

            var updatedCertificate = vm.GetCertificateFromViewModel(certificate, certData);

            await CertificateApiClient.UpdateCertificate(new UpdateCertificateRequest(updatedCertificate) { Username = username, Action = action});

            Logger.LogInformation($"Certificate for CertificateOptionViewModel requested by {username} with Id {certificate.Id} updated.");

            if (SessionService.Exists("redirecttocheck") && bool.Parse(SessionService.Get("redirecttocheck")))
            {
                Logger.LogInformation($"Certificate for CertificateOptionViewModel requested by {username} with Id {certificate.Id} redirecting back to Certificate Check.");
                return new RedirectToActionResult("Check", "CertificateCheck", null);
            }

            Logger.LogInformation($"Certificate for CertificateOptionViewModel requested by {username} with Id {certificate.Id} redirecting to {nextAction.ControllerName} {nextAction.ActionName}");
            return nextAction;
        }

        private string GetModelValues<T>(T viewModel)
        {
            var properties = typeof(T).GetProperties().ToList();

            return properties.Aggregate("", (current, prop) => current + $"{prop.Name}: {prop.GetValue(viewModel)}, ");
        }

        [HttpGet("back", Name = "CertificateOptionBack")]
        public IActionResult Back()
        {
            var username = ContextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;
            var sessionString = SessionService.Get(nameof(CertificateSession));
            if (sessionString == null)
            {
                Logger.LogInformation($"Session for CertificateOptionViewModel requested by {username} has been lost. Redirecting to Search Index");
                return RedirectToAction("Index", "Search");
            }
            var certSession = JsonConvert.DeserializeObject<CertificateSession>(sessionString);

            var hasVersions = certSession.Versions != null && certSession.Versions.Any();

            if (hasVersions)
            {
                //redirectToCheck returns you to the check certificate page
                //redirectToVersion is where a user has gone to the check page, selected change version
                //has changed version, then has to pick a new option as well if they exist
                //as a result redirecttocheck is passed so on selection of option the user goes back to the check page
                //however if a user selects back, they must go back to the version select page.
                //If a user on the check page changes just the option, then that should just go 
                //back to the check page.
                //redirectfromversion is always set when coming from version page to allow for this
                //however it's removed beyond that page, therefore if coming back from declaration
                //the final version count check allows that fallback.
                var redirectToCheck = SessionService.Exists("redirecttocheck") && bool.Parse(SessionService.Get("redirecttocheck"));
                var redirectedFromVersion = SessionService.Exists("redirectedfromversion") && bool.Parse(SessionService.Get("redirectedfromversion"));
                SessionService.Remove("redirectedfromversion");
                object routeValues = null;
                if (redirectToCheck)
                {
                    routeValues = new { redirecttocheck = true };
                }

                if (redirectedFromVersion)
                {
                    return RedirectToAction("Version", "CertificateVersion", routeValues);
                }
                else if (redirectToCheck)
                {
                    return RedirectToAction("Check", "CertificateCheck");
                }
                else if (certSession.Versions.Count > 1)
                {
                    return RedirectToAction("Version", "CertificateVersion");
                }
                else if (certSession.Versions.Count == 1)
                {
                    return RedirectToAction("Result", "Search");
                }
            }

            // No Version in Session, something wrong, return to search.
            return RedirectToAction("Index", "Search");
        }
    }
}