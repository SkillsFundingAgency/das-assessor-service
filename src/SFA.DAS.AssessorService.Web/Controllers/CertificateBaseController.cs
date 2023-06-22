using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [CheckSession]
    [Authorize]
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
        protected async Task<IActionResult> LoadViewModel<T>(string view, bool returnView = true) where T : CertificateBaseViewModel, new()
        {
            var username = GetUsernameFromClaim();

            Logger.LogInformation($"Load View Model for {typeof(T).Name} for {username}");

            var viewModel = new T();

            CheckAndSetRedirectToCheck(viewModel);

            if (!TryGetCertificateSession(typeof(T).Name, username, out var certSession))
            {
                return RedirectToAction("Index", "Search");
            }

            var certificate = await CertificateApiClient.GetCertificate(certSession.CertificateId);

            Logger.LogInformation($"Got Certificate for {typeof(T).Name} requested by {username} with Id {certificate.Id}");

            viewModel.FromCertificate(certificate);

            Logger.LogInformation($"Got View Model of type {typeof(T).Name} requested by {username}");

            if (!returnView && viewModel.GetType() == typeof(CertificateNamesViewModel)) { SetNamePropertyAndReturnView(viewModel as CertificateNamesViewModel); }

            return View(view, viewModel);
        }

        protected void SetNamePropertyAndReturnView(CertificateNamesViewModel viewModel)
        {
            viewModel.InputGivenNames = viewModel.GivenNames;
            viewModel.InputFamilyName = viewModel.FamilyName;
        }

        protected async Task<IActionResult> SaveViewModel<T>(T vm, string returnToIfModelNotValid, RedirectToActionResult nextAction, string action) where T : CertificateBaseViewModel
        {
            var username = GetUsernameFromClaim();

            Logger.LogDebug($"Save View Model for {typeof(T).Name} for {username} with values: {GetModelValues(vm)}");

            var certificate = await GetCertificate(vm.Id);
            var certData = GetCertificateData(certificate);

            if (!ModelState.IsValid)
            {
                vm.FamilyName = certData.LearnerFamilyName;
                vm.GivenNames = certData.LearnerGivenNames;
                Logger.LogDebug($"Model State not valid for {typeof(T).Name} requested by {username} with Id {certificate.Id}. Errors: {ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)}");
                return View(returnToIfModelNotValid, vm);
            }

            var updatedCertificate = vm.GetCertificateFromViewModel(certificate, certData);
            if (updatedCertificate.Status == Domain.Consts.CertificateStatus.Deleted)
            {
                updatedCertificate.Status = Domain.Consts.CertificateStatus.Draft;
                if (updatedCertificate.IsPrivatelyFunded)
                    updatedCertificate.PrivatelyFundedStatus = null;
            }

            try
            {
                await CertificateApiClient.UpdateCertificate(new UpdateCertificateRequest(updatedCertificate) { Username = username, Action = action });
            }
            catch
            {
                Logger.LogError($"Unable to update certificate with Id {certificate.Id}.");
                return RedirectToAction("Error", "Home");
            }

            Logger.LogDebug($"Certificate for {typeof(T).Name} requested by {username} with Id {certificate.Id} updated.");

            if(SessionService.GetRedirectToCheck())
            {
                Logger.LogDebug($"Certificate for {typeof(T).Name} requested by {username} with Id {certificate.Id} redirecting back to Certificate Check.");
                return new RedirectToActionResult("Check", "CertificateCheck", null);
            }

            Logger.LogDebug($"Certificate for {typeof(T).Name} requested by {username} with Id {certificate.Id} redirecting to {nextAction.ControllerName} {nextAction.ActionName}");
            return nextAction;
        }

        protected bool TryGetCertificateSession(string model, string username, out CertificateSession certSession)
        {
            var sessionString = SessionService.Get(nameof(CertificateSession));
            if (sessionString == null)
            {
                Logger.LogInformation($"Session for {model} requested by {username} has been lost. Redirecting to Search Index");
                certSession = null;
                return false;
            }

            certSession = JsonConvert.DeserializeObject<CertificateSession>(sessionString);
            return certSession != null;
        }

        protected void CheckAndSetRedirectToCheck<T>(T viewModel) where T : CertificateBaseViewModel
        {
            var query = ContextAccessor.HttpContext.Request.Query;
            if (query.ContainsKey("redirecttocheck") && bool.Parse(query["redirecttocheck"]))
            {
                SessionService.SetRedirectToCheck(true);
                viewModel.BackToCheckPage = true;
            }
            else
                SessionService.SetRedirectToCheck(false);
        }

        protected async Task<Certificate> GetCertificate(Guid certificateId)
        {
            return await CertificateApiClient.GetCertificate(certificateId);
        }

        protected async Task<CertificateData> GetCertificateData(Guid certificateId)
        {
            var certificate = await GetCertificate(certificateId);
            return GetCertificateData(certificate);
        }

        protected CertificateData GetCertificateData(Certificate certificate)
        {
            return JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
        }

        private string GetModelValues<T>(T viewModel)
        {
            var properties = typeof(T).GetProperties().ToList();

            return properties.Aggregate("", (current, prop) => current + $"{prop.Name}: {prop.GetValue(viewModel)}, ");
        }

        protected string GetUsernameFromClaim()
        {
            return GetClaimValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn");
        }

        protected string GetEpaOrgIdFromClaim()
        {
            return GetClaimValue("http://schemas.portal.com/epaoid");
        }

        private string GetClaimValue(string key)
        {
            return ContextAccessor.HttpContext.User.FindFirst(key)?.Value;
        }
    }
}