using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    [Route("certificate/date")]
    public class CertificateDateController : Controller
    {
        private readonly ILogger<CertificateController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICertificateApiClient _certificateApiClient;

        public CertificateDateController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor, ICertificateApiClient certificateApiClient)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _certificateApiClient = certificateApiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Date()
        {
            var sessionString = _contextAccessor.HttpContext.Session.GetString("CertificateSession");
            if (sessionString == null)
            {
                return RedirectToAction("Index", "Search");
            }
            var certSession = JsonConvert.DeserializeObject<CertificateSession>(sessionString);

            var certificate = await _certificateApiClient.GetCertificate(certSession.CertificateId);

            var certificateDateViewModel = new CertificateDateViewModel(certificate);

            return View("~/Views/Certificate/Date.cshtml", certificateDateViewModel);
        }

        [HttpPost(Name = "Date")]
        public async Task<IActionResult> Date(CertificateDateViewModel vm)
        {
            var certificate = await _certificateApiClient.GetCertificate(vm.Id);

            var certData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);

            if (!ModelState.IsValid)
            {
                vm.FamilyName = certData.LearnerFamilyName;
                vm.GivenNames = certData.LearnerGivenNames;
                return View("~/Views/Certificate/Date.cshtml", vm);
            }

            certData.AchievementDate = new DateTime(int.Parse(vm.Year), int.Parse(vm.Month), int.Parse(vm.Day));

            certificate.CertificateData = JsonConvert.SerializeObject(certData);

            var username = _contextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            await _certificateApiClient.UpdateCertificate(new UpdateCertificateRequest(certificate) { Username = username });

            
            return RedirectToAction("Options");
        }
    }
}