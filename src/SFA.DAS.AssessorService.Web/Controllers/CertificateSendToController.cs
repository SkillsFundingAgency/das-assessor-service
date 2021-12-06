using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Validators;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [Route("certificate/sendto")]
    public class CertificateSendToController : CertificateBaseController
    {
        private readonly CertificateSendToViewModelValidator _validator;

        public CertificateSendToController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient, CertificateSendToViewModelValidator validator, ISessionService sessionService) : base(logger, contextAccessor, certificateApiClient, sessionService)
        {
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> SendTo()
        {
            return await LoadViewModel<CertificateSendToViewModel>("~/Views/Certificate/SendTo.cshtml");
        }
        
        [HttpPost(Name = "SendTo")]
        public async Task<IActionResult> SendTo(CertificateSendToViewModel vm)
        {
            var result = _validator.Validate(vm);

            if (!result.IsValid && result.Errors.Any(e => e.Severity == Severity.Warning))
            {
                return View("~/Views/Certificate/SendTo.cshtml", vm);
            }

            var nextAction = vm.SendTo == CertificateSendTo.Apprentice
                ? RedirectToAction("Address", "CertificateAddress")
                : RedirectToAction("PreviousAddress", "CertificateAddress");

            var actionResult = await SaveViewModel(vm, 
                returnToIfModelNotValid: "~/Views/Certificate/SendTo.cshtml",
                nextAction: nextAction, action: CertificateActions.SendTo);

            return actionResult;
        }
    }
}