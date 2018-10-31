using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [Route("certificate/check")]
    public class CertificateCheckController : CertificateBaseController
    {
        public CertificateCheckController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient, ISessionService sessionService) : base(logger, contextAccessor,
            certificateApiClient, sessionService)
        {
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

        [HttpGet("CheckForRejectedApprovals/{certificateId}")]
        public async Task<IActionResult> CheckForRejectedApprovals(Guid certificateId)
        {
            var viewModel = new CertificateCheckViewModel();
            var certificate = await CertificateApiClient.GetCertificate(certificateId);
            viewModel.FromCertificate(certificate);

            TempData["HideOption"] = false;

//          return await LoadViewModel<CertificateCheckViewModel>("~/Views/Certificate/Check.cshtml");            
            return View("~/Views/Certificate/Check.cshtml", viewModel);
        }

        [HttpPost(Name = "Check")]
        public async Task<IActionResult> Check(CertificateCheckViewModel vm)
        {
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/Certificate/Check.cshtml",
                nextAction: RedirectToAction("Confirm", "CertificateConfirmation"), action: CertificateActions.Submit);
        }
    }
}