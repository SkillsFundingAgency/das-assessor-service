using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Client.Exceptions;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
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
            bool? redirectToCheck = false)
        {
            var actionResult = await LoadViewModel<CertificateAddressViewModel>("~/Views/Certificate/Address.cshtml");
            if(actionResult is ViewResult viewResult && viewResult.Model is CertificateAddressViewModel viewModel)
            {
                viewModel.EditForm = edit;
            }

            return actionResult;
        }

        [HttpPost(Name = "Address")]
        [Route("enter")]
        public async Task<IActionResult> Address(CertificateAddressViewModel vm)
        {
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
                    nextAction: RedirectToAction("Recipient", "CertificateRecipient"),
                    action: CertificateActions.Address);
            }
        }

        [HttpGet]
        [Route("select-previous")]
        public async Task<IActionResult> PreviousAddress()
        {
            var epaOrgId = GetEpaOrgIdFromClaim();
            
            var actionResult = await LoadViewModel<CertificateAddressViewModel>("~/Views/Certificate/PreviousAddress.cshtml");
            if (actionResult is ViewResult viewResult && viewResult.Model is CertificateAddressViewModel viewModel)
            {
                viewModel = await InitialisePreviousAddresssesForViewModel(viewModel, epaOrgId);
                if(!viewModel.HasPreviousAddress)
                {
                    actionResult = await LoadViewModel<CertificateAddressViewModel>("~/Views/Certificate/Address.cshtml");
                }
            }

            return actionResult;
        }

        [HttpPost(Name = "PreviousAddress")]
        [Route("select-previous")]
        public async Task<IActionResult> PreviousAddress(CertificateAddressViewModel vm)
        {
            // Could go to Recipient or to the Address Search
            return null;
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

        private async Task<ViewResult> InitialisePreviousAddresssForView(IActionResult certificateAddressViewModel, string username)
        {
            var viewResult = certificateAddressViewModel as ViewResult;
            var certificateAddress = viewResult.Model as CertificateAddressViewModel;

            await InitialisePreviousAddresses(username, certificateAddress);

            return viewResult;
        }

        private async Task<CertificateAddressViewModel> InitialisePreviousAddresssesForViewModel(CertificateAddressViewModel certificateAddressViewModel, string username)
        {
            await InitialisePreviousAddresses(username, certificateAddressViewModel);

            return certificateAddressViewModel;
        }

        private async Task InitialisePreviousAddresses(string username, CertificateAddressViewModel certificateAddress)
        {
            try
            {
                var certificatePreviousAddress = await _certificateApiClient.GetContactPreviousAddress(username);
            }
            catch (EntityNotFoundException)
            {
            }
        }
    }
}