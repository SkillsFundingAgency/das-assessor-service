﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    public class CertificateBaseController : Controller
    {
        private readonly ILogger<CertificateController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICertificateApiClient _certificateApiClient;

        public CertificateBaseController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor, ICertificateApiClient certificateApiClient)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _certificateApiClient = certificateApiClient;
        }
        protected async Task<IActionResult> LoadViewModel<T>(string view) where T : ICertificateViewModel, new()
        {
            var query = _contextAccessor.HttpContext.Request.Query;
            if (query.ContainsKey("redirecttocheck") && bool.Parse(query["redirecttocheck"]))
                _contextAccessor.HttpContext.Session.SetString("redirecttocheck", "true");
            else
                _contextAccessor.HttpContext.Session.Remove("redirecttocheck");

            var sessionString = _contextAccessor.HttpContext.Session.GetString("CertificateSession");
            if (sessionString == null)
            {
                return RedirectToAction("Index", "Search");
            }
            var certSession = JsonConvert.DeserializeObject<CertificateSession>(sessionString);

            var certificate = await _certificateApiClient.GetCertificate(certSession.CertificateId);

            var viewModel = new T();
            viewModel.FromCertificate(certificate);

            return View(view, viewModel);
        }

        protected async Task<IActionResult> SaveViewModel<T>(T vm, string returnToIfModelNotValid, RedirectToActionResult nextAction) where T : ICertificateViewModel
        {
            var certificate = await _certificateApiClient.GetCertificate(vm.Id);

            var certData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);

            if (!ModelState.IsValid)
            {
                vm.FamilyName = certData.LearnerFamilyName;
                vm.GivenNames = certData.LearnerGivenNames;
                return View(returnToIfModelNotValid, vm);
            }

            var updatedCertificate = vm.GetCertificateFromViewModel(certificate, certData);

            var username = _contextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            await _certificateApiClient.UpdateCertificate(new UpdateCertificateRequest(updatedCertificate) { Username = username });

            var session = _contextAccessor.HttpContext.Session;
            if (session.Keys.Any(k => k == "redirecttocheck") && bool.Parse(session.GetString("redirecttocheck")))
                return new RedirectToActionResult("Check", "CertificateCheck", null);
            
            return nextAction;
        }
    }
}