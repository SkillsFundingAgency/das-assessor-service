﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    public class CertificateOptionController : CertificateBaseController
    {
        public CertificateOptionController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient)
            : base(logger, contextAccessor, apiClient)
        { }


        [HttpGet]
        public async Task<IActionResult> Option(Guid certificateId, bool fromApproval)
        {
            var viewModel =  await LoadViewModel<CertificateOptionViewModel>(certificateId, "~/Views/CertificateAmend/Option.cshtml");
            if (viewModel is ViewResult viewResult && viewResult.Model is CertificateOptionViewModel certificateOptionViewModel)
                certificateOptionViewModel.FromApproval = fromApproval;

            return viewModel;
        }

        [HttpPost(Name = "Option")]
        public async Task<IActionResult> Option(CertificateOptionViewModel vm)
        {
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmend/Option.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend", new { certificateId = vm.Id, fromapproval = vm.FromApproval }), action: CertificateActions.Option);
        }
    }
}