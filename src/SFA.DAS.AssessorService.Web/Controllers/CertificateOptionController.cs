﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [Route("certificate/option")]
    public class CertificateOptionController : CertificateBaseController
    {
        public CertificateOptionController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor, ICertificateApiClient certificateApiClient, ISessionService sessionService)
            :base(logger, contextAccessor, certificateApiClient, sessionService)
        {}

        [HttpGet]
        public async Task<IActionResult> Option(bool? redirectToCheck = false)
        {
            return await LoadViewModel<CertificateOptionViewModel>("~/Views/Certificate/Option.cshtml");
        }

        [HttpPost(Name = "Option")]
        public async Task<IActionResult> Option(CertificateOptionViewModel vm)
        {
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/Certificate/Option.cshtml",
                nextAction: RedirectToAction("Date", "CertificateDate"), action: CertificateActions.Option);
        }
    }
}