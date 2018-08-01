using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.Validators;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    [Authorize]
    [Route("certificate/date")]
    public class CertificateDateController : CertificateBaseController
    {
        private readonly CertificateDateViewModelValidator _validator;

        public CertificateDateController(ILogger<CertificateAmmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient) : base(logger, contextAccessor, apiClient)
        {

        }

        [HttpGet]
        public async Task<IActionResult> Date(Guid certificateid)
        {
            return await LoadViewModel<CertificateDateViewModel>(certificateid, "~/Views/CertificateAmmend/Date.cshtml");
        }
        
        [HttpPost(Name = "Date")]
        public async Task<IActionResult> Date(CertificateDateViewModel vm)
        {
            var result = _validator.Validate(vm);

            if (!result.IsValid && result.Errors.Any(e => e.Severity == Severity.Warning))
            {
                vm.WarningShown = "true";
                return View("~/Views/CertificateAmmend/Date.cshtml", vm);
            }


            var actionResult = await SaveViewModel(vm, 
                returnToIfModelNotValid: "~/Views/Certificate/Date.cshtml",
                nextAction: RedirectToAction("Address", "CertificateAddress"), action: CertificateActions.Date);


            return actionResult;
        }
    }
}