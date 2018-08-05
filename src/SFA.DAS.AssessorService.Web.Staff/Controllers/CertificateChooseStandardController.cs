using System;
using System.Threading.Tasks;
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
        public async Task<IActionResult> ChooseStandard(Guid certificateid)
        {
            return await LoadViewModel<CertificateGradeViewModel>(certificateid, "~/Views/CertificateAmmend/Grade.cshtml");
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