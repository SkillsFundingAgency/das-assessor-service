using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{   
    public class CertificateAddressController : CertificateBaseController
    {
        public CertificateAddressController(ILogger<CertificateAmmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient)
            : base(logger, contextAccessor, apiClient)
        { }

        [HttpGet]
        public async Task<IActionResult> Address(Guid certificateid)
        {
            return await LoadViewModel<CertificateAddressViewModel>(certificateid, "~/Views/CertificateAmmend/Address.cshtml");
        }

        [HttpPost(Name = "Grade")]
        public async Task<IActionResult> Address(CertificateAddressViewModel vm)
        {
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmmend/Address.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmmend", new { certificateid = vm.Id }), action: CertificateActions.Address);
        }
    }
}