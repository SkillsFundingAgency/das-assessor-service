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
    [Route("certificate/grade")]
    public class CertificateGradeController : CertificateBaseController
    {
        public CertificateGradeController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor, ICertificateApiClient certificateApiClient, ISessionService sessionService)
            : base(logger, contextAccessor, certificateApiClient, sessionService)
        {}

        [HttpGet]
        public async Task<IActionResult> Grade(bool? redirectToCheck = false)
        {
            return await LoadViewModel<CertificateGradeViewModel>("~/Views/Certificate/Grade.cshtml");
        }

        [HttpPost(Name="Grade")]
        public async Task<IActionResult> Grade(CertificateGradeViewModel vm)
        {
            var certificate = await CertificateApiClient.GetCertificate(vm.Id);
            if (certificate.IsPrivatelyFunded)
            {
                return await SaveViewModel(vm,
                    returnToIfModelNotValid: "~/Views/Certificate/Grade.cshtml",
                    nextAction: RedirectToAction("LearnerStartDate", "CertificatePrivateLearnerStartDate"),
                    action: CertificateActions.Grade);
            }
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/Certificate/Grade.cshtml",
                nextAction: RedirectToAction("Date", "CertificateDate"), action: CertificateActions.Grade);
        }

    }
}