﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        protected async Task<IActionResult> LoadViewModel<T>(string view) where T : CertificateBaseViewModel, new()
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

            return View(view, viewModel);
        }

        protected async Task<IActionResult> SaveViewModel<T>(T vm, string returnToIfModelNotValid, RedirectToActionResult nextAction, string action) where T : CertificateBaseViewModel
        {
            var username = GetUsernameFromClaim();

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
            if (updatedCertificate.Status == Domain.Consts.CertificateStatus.Deleted)
            {
                updatedCertificate.Status = Domain.Consts.CertificateStatus.Draft;
                if (updatedCertificate.IsPrivatelyFunded)
                    updatedCertificate.PrivatelyFundedStatus = null;
            }

            await CertificateApiClient.UpdateCertificate(new UpdateCertificateRequest(updatedCertificate) { Username = username, Action = action });

            Logger.LogInformation($"Certificate for {typeof(T).Name} requested by {username} with Id {certificate.Id} updated.");

            if (SessionService.TryGet<bool>("redirecttocheck", out var redirectToCheck) && redirectToCheck)
            {
                if (nextAction.ActionName == "AddressSummary")
                {
                    var certAddress = vm as CertificateAddressViewModel;
                    if (string.IsNullOrEmpty(certAddress.Employer))
                    {
                        return new RedirectToActionResult("AddressSummary", "CertificateAddressSummary", new { redirecttocheck = "true" });
                    }
                }

                Logger.LogInformation(
                    $"Certificate for {typeof(T).Name} requested by {username} with Id {certificate.Id} redirecting back to Certificate Check.");
                return new RedirectToActionResult("Check", "CertificateCheck", null);
            }

            Logger.LogInformation($"Certificate for {typeof(T).Name} requested by {username} with Id {certificate.Id} redirecting to {nextAction.ControllerName} {nextAction.ActionName}");
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
                Logger.LogInformation($"RedirectToCheck for {typeof(T).Name} is true");
                SessionService.Set("redirecttocheck", "true");
                viewModel.BackToCheckPage = true;
            }
            else
                SessionService.Remove("redirecttocheck");
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