using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [Route("certificate/grade")]
    public class CertificateGradeController : Controller
    {
        private readonly ILogger<CertificateController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICertificateApiClient _certificateApiClient;

        public CertificateGradeController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor, ICertificateApiClient certificateApiClient)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _certificateApiClient = certificateApiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Grade()
        {
            var sessionString = _contextAccessor.HttpContext.Session.GetString("CertificateSession");
            if (sessionString == null)
            {
                return RedirectToAction("Index", "Search");
            }
            var certSession = JsonConvert.DeserializeObject<CertificateSession>(sessionString);

            var certificate = await _certificateApiClient.GetCertificate(certSession.CertificateId);

            var certificateGradeViewModel = new CertificateGradeViewModel(certificate);

            certificateGradeViewModel.SetUpGrades();

            return View("~/Views/Certificate/Grade.cshtml", certificateGradeViewModel);
        }

        [HttpPost(Name="Grade")]
        public async Task<IActionResult> Grade(CertificateGradeViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.SetUpGrades();
                return View("~/Views/Certificate/Grade.cshtml", vm);
            }

            var certificate = await _certificateApiClient.GetCertificate(vm.Id);

            var certData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);

            certData.OverallGrade = vm.SelectedGrade;

            certificate.CertificateData = JsonConvert.SerializeObject(certData);

            var username = _contextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            await _certificateApiClient.UpdateCertificate(new UpdateCertificateRequest(certificate){Username = username});
            
            return RedirectToAction("Option", "CertificateOption");
        }
    }
}