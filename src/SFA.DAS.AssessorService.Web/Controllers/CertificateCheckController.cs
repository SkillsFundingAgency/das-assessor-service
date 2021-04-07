using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Validators;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [Route("certificate/check")]
    public class CertificateCheckController : CertificateBaseController
    {
        private readonly CertificateCheckViewModelValidator _validator;

        public CertificateCheckController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient, CertificateCheckViewModelValidator validator, IStandardServiceClient standardServiceClient, ISessionService sessionService) : base(logger, contextAccessor, certificateApiClient, standardServiceClient, sessionService)
        {
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> Check()
        {
            var sessionString = SessionService.Get("CertificateSession");
            if (sessionString == null)
            {
                return RedirectToAction("Index", "Search");
            }
            var certSession = JsonConvert.DeserializeObject<CertificateSession>(sessionString);
            TempData["HideOption"] = !certSession.Options.Any();

            return await LoadViewModel<CertificateCheckViewModel>("~/Views/Certificate/Check.cshtml");
        }
        
        [HttpPost(Name = "Check")]
        public async Task<IActionResult> Check(CertificateCheckViewModel vm)
        {
            // This is the final step in the process so get the latest version of the certificate!
            if (vm is null) vm = new CertificateCheckViewModel();
            vm.FromCertificate(await CertificateApiClient.GetCertificate(vm.Id));

            var result = _validator.Validate(vm);

            if (!result.IsValid)
            {
                return await Check();
            }

            return await SaveViewModel(vm, 
                returnToIfModelNotValid: "~/Views/Certificate/Check.cshtml",
                nextAction: RedirectToAction("Confirm", "CertificateConfirmation"), action: CertificateActions.Submit);
        }
    }
}