using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [CheckSession]
    public class CertificateBaseController : Controller
    {
        protected readonly ILogger<CertificateController> Logger;
        protected readonly IHttpContextAccessor ContextAccessor;
        protected readonly ICertificateApiClient CertificateApiClient;
        protected readonly ISessionService SessionService;

        public CertificateBaseController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor, ICertificateApiClient certificateApiClient, ISessionService sessionService)
        {
            Logger = logger;
            ContextAccessor = contextAccessor;
            CertificateApiClient = certificateApiClient;
            SessionService = sessionService;
        }
        protected async Task<IActionResult> LoadViewModel<T>(string view) where T : ICertificateViewModel, new()
        {
            var username = ContextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            Logger.LogInformation($"Load View Model for {typeof(T).Name} for {username}");
            
            var viewModel = new T();

            var query = ContextAccessor.HttpContext.Request.Query;
            if (query.ContainsKey("redirecttocheck") && bool.Parse(query["redirecttocheck"]))
            {
                Logger.LogInformation($"RedirectToCheck for {typeof(T).Name} is true");
                SessionService.Set("redirecttocheck", "true");
                viewModel.BackToCheckPage = true;
            }
            else
                SessionService.Remove("redirecttocheck");
            
            if (query.ContainsKey("redirecttosearch") && bool.Parse(query["redirecttosearch"]))
            {
                Logger.LogInformation($"RedirectToSearch for {typeof(T).Name} is true");
                SessionService.Set("redirecttosearch", "true");
                viewModel.BackToSearchPage = true;
            }
            else
                SessionService.Remove("redirecttosearch");
            
                
            var sessionString = SessionService.Get("CertificateSession");
            if (sessionString == null)
            {
                Logger.LogInformation($"Session for {typeof(T).Name} requested by {username} has been lost. Redirecting to Search Index");
                return RedirectToAction("Index", "Search");
            }
            var certSession = JsonConvert.DeserializeObject<CertificateSession>(sessionString);

            var certificate = await CertificateApiClient.GetCertificate(certSession.CertificateId);

            Logger.LogInformation($"Got Certificate for {typeof(T).Name} requested by {username} with Id {certificate.Id}");

            viewModel.FromCertificate(certificate);

            Logger.LogInformation($"Got View Model of type {typeof(T).Name} requested by {username}");

            return View(view, viewModel);
        }

        protected async Task<IActionResult> SaveViewModel<T>(T vm, string returnToIfModelNotValid, RedirectToActionResult nextAction, string action) where T : ICertificateViewModel
        {
            var username = ContextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;
            
            Logger.LogInformation($"Save View Model for {typeof(T).Name} for {username} with values: {GetModelValues(vm)}");

            var certificate = await CertificateApiClient.GetCertificate(vm.Id);
            var certData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);

            if (!ModelState.IsValid)
            {
                vm.FamilyName = certData.LearnerFamilyName;
                vm.GivenNames = certData.LearnerGivenNames;
                Logger.LogInformation($"Model State not valid for {typeof(T).Name} requested by {username} with Id {certificate.Id}. Errors: {ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)}");
                return View(returnToIfModelNotValid, vm);
            }

            var updatedCertificate = vm.GetCertificateFromViewModel(certificate, certData);

            await CertificateApiClient.UpdateCertificate(new UpdateCertificateRequest(updatedCertificate) { Username = username, Action = action});

            Logger.LogInformation($"Certificate for {typeof(T).Name} requested by {username} with Id {certificate.Id} updated.");

            if (SessionService.Exists("redirecttocheck") && bool.Parse(SessionService.Get("redirecttocheck")))                              
            {
                Logger.LogInformation($"Certificate for {typeof(T).Name} requested by {username} with Id {certificate.Id} redirecting back to Certificate Check.");
                return new RedirectToActionResult("Check", "CertificateCheck", null);
            }
            
            if( SessionService.Exists("redirecttosearch") && bool.Parse(SessionService.Get("redirecttosearch")))
            {
                Logger.LogInformation($"Certificate for {typeof(T).Name} requested by {username} with Id {certificate.Id} redirecting back to Certificate Check.");
                return new RedirectToActionResult("CheckForRejectedApprovals", "CertificateCheck", new { certificateId = certificate.Id });
            }

            Logger.LogInformation($"Certificate for {typeof(T).Name} requested by {username} with Id {certificate.Id} redirecting to {nextAction.ControllerName} {nextAction.ActionName}");
            return nextAction;
        }

        private string GetModelValues<T>(T viewModel)
        {
            var properties = typeof(T).GetProperties().ToList();

            return properties.Aggregate("", (current, prop) => current + $"{prop.Name}: {prop.GetValue(viewModel)}, ");
        }
    }
}