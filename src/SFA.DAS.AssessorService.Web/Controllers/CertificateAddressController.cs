using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
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
            certificateAddressViewModel = await InitialisePreviousAddresssesForView(certificateAddressViewModel, username);

            return certificateAddressViewModel;
        }

        [HttpGet("newaddress", Name="NewAddress")]
        public async Task<IActionResult> NewAddress(bool? redirectToCheck = false)
        {
            var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")?.Value;
            var username = _contextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            var viewModel = await LoadViewModel<CertificateAddressViewModel>("~/Views/Certificate/Address.cshtml");
            var viewResult = viewModel as ViewResult;
            var certificateAddress = viewResult.Model as CertificateAddressViewModel;

            certificateAddress.AddressLine1 = String.Empty;
            certificateAddress.AddressLine2 = String.Empty;
            certificateAddress.AddressLine3 = String.Empty;
            certificateAddress.City = String.Empty;
            certificateAddress.Postcode = String.Empty;

            var certificateAddressViewModel = View("~/Views/Certificate/Address.cshtml", certificateAddress);
            certificateAddressViewModel = await InitialisePreviousAddresssesForView(certificateAddressViewModel, username);

            return certificateAddressViewModel;
        }

        [HttpPost(Name = "Address")]
        public async Task<IActionResult> Address(CertificateAddressViewModel vm)
        {
            var username = _contextAccessor.HttpContext.User
                .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            if (vm.SelectPreviousAddress)
            {             
                var certificatePreviousAddress = await _certificateApiClient.GetContactPreviousAddress(username);
                vm = UpdateViewModelWithPreviousAddress(vm, certificatePreviousAddress);             
            }

            if (!ModelState.IsValid)
            {             
                vm = await InitialisePreviousAddresssesForViewModel(vm, username);
            }

            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/Certificate/Address.cshtml",
                nextAction: RedirectToAction("AddressSummary", "CertificateAddressSummary"), action: CertificateActions.Address);
        }

        private CertificateAddressViewModel UpdateViewModelWithPreviousAddress(CertificateAddressViewModel vm,
            CertificateAddress certificatePreviousAddress)
        {
            vm.AddressLine1 = certificatePreviousAddress.AddressLine1;
            vm.AddressLine2 = certificatePreviousAddress.AddressLine2;
            vm.AddressLine3 = certificatePreviousAddress.AddressLine3;
            vm.City = certificatePreviousAddress.City;
            vm.Postcode = certificatePreviousAddress.PostCode;

            return vm;
        }

        private async Task<ViewResult> InitialisePreviousAddresssesForView(IActionResult certificateAddressViewModel,
            string  username)
        {
            var certificatePreviousAddress = await _certificateApiClient.GetContactPreviousAddress(username);
            var certificatePreviousAddresses = await _certificateApiClient.GetPreviousAddressess(username);
            
            var viewResult = certificateAddressViewModel as ViewResult;
            var certificateAddress = viewResult.Model as CertificateAddressViewModel;
            certificateAddress.CertificateContactPreviousAddress = new CertificatePreviousAddressViewModel(certificatePreviousAddress);
            certificateAddress.CertificatePreviousAddressViewModels =
                certificatePreviousAddresses.Select(q => new CertificatePreviousAddressViewModel(q)).ToList();

            return viewResult;
        }

        private async Task<CertificateAddressViewModel> InitialisePreviousAddresssesForViewModel(CertificateAddressViewModel certificateAddressViewModel,
            string username)
        {
            var certificatePreviousAddress = await _certificateApiClient.GetContactPreviousAddress(username);
            var certificatePreviousAddresses = await _certificateApiClient.GetPreviousAddressess(username);

            certificateAddressViewModel.CertificateContactPreviousAddress = new CertificatePreviousAddressViewModel(certificatePreviousAddress);
            certificateAddressViewModel.CertificatePreviousAddressViewModels =
                certificatePreviousAddresses.Select(q => new CertificatePreviousAddressViewModel(q)).ToList();

            return certificateAddressViewModel;
        }
    }
}