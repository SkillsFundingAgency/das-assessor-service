﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    public class CertificateAmmendController : CertificateBaseController
    {
        public CertificateAmmendController(ILogger<CertificateAmmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient) : base(logger, contextAccessor, apiClient)
        {

        }

        [HttpGet]
        public async Task<IActionResult> Check(Guid certificateid)
        {
            return await LoadViewModel<CertificateCheckViewModel>(certificateid, "~/Views/CertificateAmmend/Check.cshtml");
        }     

        [HttpPost(Name = "Check")]
        public async Task<IActionResult> ConfirmAndSubmit(CertificateCheckViewModel vm)
        {
            return RedirectToAction("Index", "DuplicateRequest", new { certificateid = vm.Id });
        }
    }
}