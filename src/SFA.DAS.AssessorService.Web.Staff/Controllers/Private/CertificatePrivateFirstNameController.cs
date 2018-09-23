using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Private;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers.Private
{
    [Authorize]
    [Route("certificate/firstname")]
    public class CertificatePrivateFirstNameController : CertificateBaseController
    {
        public CertificatePrivateFirstNameController(ILogger<CertificateAmmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient) : base(logger, contextAccessor, apiClient)
        { }

        [HttpGet]
        public async Task<IActionResult> FirstName(Guid certificateid)
        {
            CertificateFirstNameViewModel vm = new CertificateFirstNameViewModel();
            return await LoadViewModel<CertificateFirstNameViewModel>(certificateid, "~/Views/CertificateAmmend/FirstName.cshtml");
        }

        [HttpPost(Name = "FirstName")]
        public async Task<IActionResult> FirstName(CertificateFirstNameViewModel vm)
        {
            var actionResult = await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmmend/FirstName.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmmend", new { certificateid = vm.Id }), action: CertificateActions.FirstName);

            return actionResult;
        }
    }
}