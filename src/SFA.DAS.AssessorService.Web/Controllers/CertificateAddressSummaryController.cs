using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [Route("certificate/addresssummary")]
    public class CertificateAddressSummaryController : CertificateBaseController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICertificateApiClient _certificateApiClient;

        public CertificateAddressSummaryController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient, ISessionService sessionService) : base(logger, contextAccessor, certificateApiClient, sessionService)
        {
            _contextAccessor = contextAccessor;
            _certificateApiClient = certificateApiClient;
        }

        [HttpGet]
        public async Task<IActionResult> AddressSummary()
        {
            var certificateAddressViewModel = await LoadViewModel<CertificateAddressViewModel>("~/Views/Certificate/AddressSummary.cshtml");
            return certificateAddressViewModel;
        }

        [HttpPost(Name = "AddressSummary")]
        public IActionResult AddressSummary(CertificateAddressViewModel vm)
        {
            return RedirectToAction("Recipient", "CertificateRecipient");
        }
    }
}