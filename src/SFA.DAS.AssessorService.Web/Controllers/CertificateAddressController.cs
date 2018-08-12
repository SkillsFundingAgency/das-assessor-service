﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Client.Exceptions;
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
        public async Task<IActionResult> Address(
            bool? edit,
            bool? reset,
            bool? redirectToCheck = false)
        {
            var username = _contextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            var certificateAddressViewModel = await LoadViewModel<CertificateAddressViewModel>("~/Views/Certificate/Address.cshtml");
            //if (AddressAlreadyInitialised(certificateAddressViewModel) && (edit ?? false) == false)
            //    return RedirectToAction("AddressSummary", "CertificateAddressSummary");

            certificateAddressViewModel = await InitialisePreviousAddresssForView(
                certificateAddressViewModel: certificateAddressViewModel, username: username);

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
                vm = vm.CopyFromCertificateAddress(certificatePreviousAddress);             
            }

            if (!ModelState.IsValid)
            {
                vm = await InitialisePreviousAddresssesForViewModel(vm, username);
            }

            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/Certificate/Recipient.cshtml",
                nextAction: RedirectToAction("Recipient", "CertificateRecipient"), action: CertificateActions.Address);
        }

        private async Task<ViewResult> InitialisePreviousAddresssForView(
            IActionResult certificateAddressViewModel,
            string username)
        {
            var viewResult = certificateAddressViewModel as ViewResult;
            var certificateAddress = viewResult.Model as CertificateAddressViewModel;

            await InitialisePreviousAddresses(username, certificateAddress);

            return viewResult;
        }

        private async Task<CertificateAddressViewModel> InitialisePreviousAddresssesForViewModel(CertificateAddressViewModel certificateAddressViewModel,
            string username)
        {
            await InitialisePreviousAddresses(username, certificateAddressViewModel);

            return certificateAddressViewModel;
        }

        private async Task InitialisePreviousAddresses(string username, CertificateAddressViewModel certificateAddress)
        {
            try
            {
                var certificatePreviousAddress = await _certificateApiClient.GetContactPreviousAddress(username);

                certificateAddress.PreviousAddress =
                    new CertificatePreviousAddressViewModel(certificatePreviousAddress);

            }
            catch (EntityNotFoundException e)
            {
                var certificatePreviousAddress = new CertificateAddress();
                certificateAddress.PreviousAddress =
                    new CertificatePreviousAddressViewModel(certificatePreviousAddress);
            }
        }     
    }
}