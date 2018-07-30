using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
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
        public async Task<IActionResult> AddressSummary(CertificateAddressViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Certificate/AddressSummary.cshtml", vm);
            }

            if (!string.IsNullOrEmpty(vm.Employer))
            {
                var certificateAddressView =
                    await LoadViewModel<CertificateAddressViewModel>("~/Views/Certificate/AddressSummary.cshtml");

                var viewResult = certificateAddressView as ViewResult;
                var certificateAddressViewModel = viewResult.Model as CertificateAddressViewModel;

                certificateAddressViewModel.Employer = vm.Employer;

                return await SaveViewModel(certificateAddressViewModel,
                    returnToIfModelNotValid: "~/Views/Certificate/AddressSummary.cshtml",
                    nextAction: RedirectToAction("Recipient", "CertificateRecipient"), action: CertificateActions.AddressSummary);
            }

            return RedirectToAction("Recipient", "CertificateRecipient");
        }
    }
}