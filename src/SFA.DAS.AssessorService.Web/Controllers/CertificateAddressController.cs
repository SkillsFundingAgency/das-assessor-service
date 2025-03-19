using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [Route("certificate/address")]
    public class CertificateAddressController : CertificateBaseController
    {
        private readonly ICertificateApiClient _certificateApiClient;

        public CertificateAddressController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient, ISessionService sessionService) : base(logger, contextAccessor, certificateApiClient, sessionService)
        {
            _certificateApiClient = certificateApiClient;
        }

        [HttpGet]
        [Route("enter")]
        public async Task<IActionResult> Address(
            bool edit = false,
            bool hasPreviousAddress = false,
            bool? redirectToCheck = false)
        {
            var actionResult = await LoadViewModel<CertificateAddressViewModel>("~/Views/Certificate/Address.cshtml");
            if(actionResult is ViewResult viewResult && viewResult.Model is CertificateAddressViewModel viewModel)
            {

                viewModel.HasPreviousAddress = hasPreviousAddress;
                viewModel.EditForm = edit;
            }

            return actionResult;
        }

        [HttpPost(Name = "Address")]
        [Route("enter")]
        public async Task<IActionResult> Address(CertificateAddressViewModel vm)
        {
            var certData = await GetCertificateData(vm.Id);
            if (ModelState.IsValid && vm.AddressHasChanged(certData))
            {
                // when address has been changed the complete journey is required
                SessionService.SetRedirectToCheck(false);
            }

            if (vm.SendTo == CertificateSendTo.Apprentice)
            {
                return await SaveViewModel(vm,
                    returnToIfModelNotValid: "~/Views/Certificate/Address.cshtml",
                    nextAction: RedirectToAction("ConfirmAddress", "CertificateAddress"),
                    action: CertificateActions.Address);
            }
            else
            {
                return await SaveViewModel(vm,
                    returnToIfModelNotValid: "~/Views/Certificate/Address.cshtml",
                    nextAction: RedirectToAction("Recipient", "CertificateAddress"),
                    action: CertificateActions.Address);
            }
        }

        [HttpGet]
        [Route("select-previous")]
        public async Task<IActionResult> PreviousAddress()
        {
            var epaOrgId = GetEpaOrgIdFromClaim();
            
            var actionResult = await LoadViewModel<CertificatePreviousAddressViewModel>("~/Views/Certificate/PreviousAddress.cshtml");
            if (actionResult is ViewResult viewResult && viewResult.Model is CertificatePreviousAddressViewModel viewModel)
            {
                await InitialisePreviousAddress(viewModel, epaOrgId);
                if(!viewModel.HasPreviousAddress)
                {
                    actionResult = RedirectToAction("Address", "CertificateAddress");
                }
            }

            return actionResult;
        }

        [HttpPost(Name = "PreviousAddress")]
        [Route("select-previous")]
        public async Task<IActionResult> PreviousAddress(CertificatePreviousAddressViewModel vm)
        {
            if (vm.UsePreviousAddress == true)
            {
                return await SaveViewModel(vm,
                    returnToIfModelNotValid: "~/Views/Certificate/PreviousAddress.cshtml",
                    nextAction: RedirectToAction("Recipient", "CertificateAddress", new { UsePreviousAddress = true }),
                    action: CertificateActions.Address);
            }
            else
            {
                return await SaveViewModel(vm,
                    returnToIfModelNotValid: "~/Views/Certificate/PreviousAddress.cshtml",
                    nextAction: RedirectToAction("Address", "CertificateAddress", new { HasPreviousAddress = true }),
                    action: CertificateActions.Address);
            }
        }

        [HttpGet]
        [Route("recipient")]
        public async Task<IActionResult> Recipient(
            bool edit = false,
            bool usePreviousAddress = false)
        {
            var actionResult = await LoadViewModel<CertificateRecipientViewModel>("~/Views/Certificate/Recipient.cshtml");
            if (actionResult is ViewResult viewResult && viewResult.Model is CertificateRecipientViewModel viewModel)
            {
                viewModel.UsePreviousAddress = usePreviousAddress;
                viewModel.EditForm = edit;
            }

            return actionResult;
        }

        [HttpPost(Name = "Recipient")]
        [Route("recipient")]
        public async Task<IActionResult> Recipient(CertificateRecipientViewModel vm)
        {
            var certData = await GetCertificateData(vm.Id);
            if (ModelState.IsValid && vm.RecipientHasChanged(certData))
            {
                // when recipient has been changed the complete journey is required
                SessionService.SetRedirectToCheck(false);
            }

            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/Certificate/Recipient.cshtml",
                nextAction: RedirectToAction("ConfirmAddress", "CertificateAddress"),
                action: CertificateActions.Address);
        }

        [HttpGet]
        [Route("confirm")]
        public async Task<IActionResult> ConfirmAddress()
        {
            var actionResult = await LoadViewModel<CertificateRecipientViewModel>("~/Views/Certificate/ConfirmAddress.cshtml");
            return actionResult;
        }
        
        [HttpPost(Name = "ConfirmAddress")]
        [Route("confirm")]
        public async Task<IActionResult> ConfirmAddress(CertificateRecipientViewModel vm)
        {
            var username = GetUsernameFromClaim();

            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/Certificate/ConfirmAddress.cshtml",
                nextAction: RedirectToAction("Check", "CertificateCheck"), action: CertificateActions.ConfirmAddress);
        }

        private async Task InitialisePreviousAddress(CertificatePreviousAddressViewModel viewModel, string epaOrgId)
        {
            var previousAddress = await GetContactPreviousAddress(epaOrgId, viewModel.EmployerAccountId);
            if (previousAddress != null)
            {
                viewModel.PreviousAddress = new CertificateAddress
                {
                    ContactOrganisation = previousAddress.ContactOrganisation,
                    AddressLine1 = previousAddress.AddressLine1,
                    AddressLine2 = previousAddress.AddressLine2,
                    AddressLine3 = previousAddress.AddressLine3,
                    City = previousAddress.City,
                    PostCode = previousAddress.PostCode
                };
            }

            viewModel.HasPreviousAddress = (previousAddress != null);
        }

        private async Task<CertificateAddress> GetContactPreviousAddress(string epaOrgId, long? employerAccountId)
        {
            try
            {
                return await _certificateApiClient.GetContactPreviousAddress(epaOrgId, employerAccountId);
            }
            catch (HttpRequestException)
            {
                // when there is no previous address NoContent is correctly returned from the API but the client
                // incorrectly throws an exception 
                return null;
            }
        }
    }
}