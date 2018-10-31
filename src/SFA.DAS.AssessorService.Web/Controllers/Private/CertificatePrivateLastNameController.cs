using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private;

namespace SFA.DAS.AssessorService.Web.Controllers.Private
{
    [Authorize]
    [Route("certificate/lastname")]
    public class CertificatePrivateLastNameController : CertificateBaseController
    {
        public CertificatePrivateLastNameController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor, ICertificateApiClient certificateApiClient, ISessionService sessionService)
            : base(logger, contextAccessor, certificateApiClient, sessionService)
        { }

        [HttpGet]
        public async Task<IActionResult> LastName(bool? redirectToCheck = false,
            bool? redirecttosearch = false)
        {            
            return await LoadViewModel<CertificateLastNameViewModel>("~/Views/Certificate/LastName.cshtml");
        }

        [HttpPost(Name = "LastName")]
        public async Task<IActionResult> LastName(CertificateLastNameViewModel vm)
        {
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/Certificate/LastName.cshtml",
                nextAction: RedirectToAction("StandardCode", "CertificatePrivateStandardCode"), action: CertificateActions.LastName);
        }
    }
}