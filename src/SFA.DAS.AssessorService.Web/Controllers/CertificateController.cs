using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [CheckSession]
    [Route("certificate")]
    public class CertificateController : Controller
    {
        private readonly ILogger<CertificateController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICertificateApiClient _certificateApiClient;
        private readonly ISessionService _sessionService;

        public CertificateController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient, ISessionService sessionService)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _certificateApiClient = certificateApiClient;
            _sessionService = sessionService;
        }

        [HttpPost]
        public async Task<IActionResult> Start(CertificateStartViewModel vm)
        {
            _sessionService.Remove("CertificateSession");
            var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")?.Value;
            var username = _contextAccessor.HttpContext.User
                .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            _logger.LogInformation(
                $"Start of Certificate Start for ULN {vm.Uln} and Standard Code: {vm.StdCode} by user {username}");

            var cert = await _certificateApiClient.Start(new StartCertificateRequest()
            {
                UkPrn = int.Parse(ukprn),
                StandardCode = vm.StdCode,
                Uln = vm.Uln,
                Username = username
            });

            var options = (await _certificateApiClient.GetOptions(cert.StandardCode)).Select(o => o.OptionName).ToList();

            _sessionService.Set("CertificateSession", new CertificateSession()
            {
                CertificateId = cert.Id,
                Uln = vm.Uln,
                StandardCode = vm.StdCode,
                Options = options
            });

            _logger.LogInformation(
                $"New Certificate received for ULN {vm.Uln} and Standard Code: {vm.StdCode} with ID {cert.Id}");

            return RedirectToAction("Declare", "CertificateDeclaration");
        }

        [HttpPost]
        [Route("certificate/private")]
        public async Task<IActionResult> StartPrivate(CertificateStartPrivateViewModel certificateStartPrivateViewModel)
        {
            _sessionService.Remove("CertificateSession");
            var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")?.Value;
            var username = _contextAccessor.HttpContext.User
                .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            _logger.LogInformation(
                $"Start of Private Certificate");

            var cert = await _certificateApiClient.StartPrivate(new StartCertificatePrivateRequest()
            {
                UkPrn = int.Parse(ukprn),
                Uln = certificateStartPrivateViewModel.Uln,
                LastName = certificateStartPrivateViewModel.Surname,
                Username = username
            });

            _sessionService.Set("CertificateSession", new CertificateSession()
            {
                CertificateId = cert.Id,
                Uln = certificateStartPrivateViewModel.Uln
            });

            _logger.LogInformation(
                $"New Private Certificate received with ID {cert.Id}");

            return RedirectToAction("FirstName", "CertificatePrivateFirstName");
        }
    }
}