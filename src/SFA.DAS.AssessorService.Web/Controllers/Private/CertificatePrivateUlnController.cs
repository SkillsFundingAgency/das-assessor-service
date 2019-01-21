using System;
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
    [Route("certificate/uln")]
    public class CertificatePrivateUlnController : CertificateBaseController
    {
        public CertificatePrivateUlnController(ILogger<CertificateController> logger,
            IHttpContextAccessor contextAccessor, ICertificateApiClient certificateApiClient, ISessionService sessionService)
            : base(logger, contextAccessor, certificateApiClient, sessionService)
        { }

        [HttpGet]
        public async Task<IActionResult> Uln(Guid certificateId)
        {
            return await LoadViewModel<CertificateUlnViewModel>("~/Views/Certificate/Uln.cshtml");
        }

        [HttpPost(Name = "Uln")]
        public async Task<IActionResult> Uln(CertificateUlnViewModel vm)
        {
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/Certificate/Uln.cshtml",
                nextAction: RedirectToAction("Uln", "CertificatePrivateUln"), action: CertificateActions.Uln);
        }
    }
}