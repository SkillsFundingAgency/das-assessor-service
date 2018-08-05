using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{   
    public class CertificateChooseStandardController : CertificateBaseController
    {
        public CertificateChooseStandardController(ILogger<CertificateAmmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient)
            : base(logger, contextAccessor, apiClient)
        { }

        [HttpGet]
        public async Task<IActionResult> ChooseStandard(SearchRequestViewModel vm)
        {
            var username = ContextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            var results = await ApiClient.SearchForStandards(new StandardViewModelRequest()
            {
                Surname = vm.Surname,
                Uln = long.Parse(vm.Uln),
                UkPrn = int.Parse(vm.Ukprn.Value.ToString()),
                Username = "jcoxhead"
            });

            vm.SearchResults = Mapper.Map<List<ResultViewModel>>(results);

            return View("~/Views/CertificateAmmend/ChooseStandard.cshtml", vm);
        }

        [HttpPost(Name = "ChooseStandard")]
        public async Task<IActionResult> ChooseStandard(CertificateGradeViewModel vm)
        {
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmmend/Grade.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmmend"), action: CertificateActions.Grade);
        }
    }
}