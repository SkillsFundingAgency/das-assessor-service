using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [Route("certificate/address")]
    public class CertificateAddressController : CertificateBaseController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICertificateApiClient _certificateApiClient;

        public CertificateAddressController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient, ISessionService sessionService) : base(logger, contextAccessor, certificateApiClient, sessionService)
        {
            _contextAccessor = contextAccessor;
            _certificateApiClient = certificateApiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Address(bool? redirectToCheck = false)
        {         
            var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")?.Value;
            var username = _contextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            var certificateAddressViewModel  = await LoadViewModel<CertificateAddressViewModel>("~/Views/Certificate/Address.cshtml");

            certificateAddressViewModel = await UpdateViewModelWithPreviousAddress(certificateAddressViewModel, username);

            return certificateAddressViewModel;
        }

        [HttpGet("newaddress", Name="NewAddress")]
        public async Task<IActionResult> NewAddress(bool? redirectToCheck = false)
        {
            var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")?.Value;
            var username = _contextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            var viewModel = new CertificateAddressViewModel();

            var certificateAddressViewModel = View("~/Views/Certificate/Address.cshtml", viewModel);
            certificateAddressViewModel = await UpdateViewModelWithPreviousAddress(certificateAddressViewModel, username);

            return certificateAddressViewModel;
        }


        [HttpPost(Name = "Address")]
        public async Task<IActionResult> Address(CertificateAddressViewModel vm)
        {
            //TempData.Put("AddressInformation", vm);
            //return RedirectToAction("AddressSummary", "CertificateAddressSummary");
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/Certificate/Address.cshtml",
                nextAction: RedirectToAction("AddressSummary", "CertificateAddressSummary"), action: CertificateActions.Address);
        }

        private async Task<ViewResult> UpdateViewModelWithPreviousAddress(IActionResult certificateAddressViewModel,
            string  username)
        {
            var certificatePreviousAddress = await _certificateApiClient.GetContactPreviousAddress(username);
            var viewResult = certificateAddressViewModel as ViewResult;
            var certificateAddress = viewResult.Model as CertificateAddressViewModel;
            certificateAddress.CertificatePreviousAddress = certificatePreviousAddress.StringifyAddress();

            return viewResult;
        }
    }
}