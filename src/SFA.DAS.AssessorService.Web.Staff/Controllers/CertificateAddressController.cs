using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        public CertificateAddressController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient)
            : base(logger, contextAccessor, apiClient)
        { }

        [HttpGet]
        public async Task<IActionResult> Address(Guid certificateId)
        {
            return await LoadViewModel<CertificateAddressViewModel>(certificateId, "~/Views/CertificateAmend/Address.cshtml");
        }

        [HttpPost(Name = "Grade")]
        public async Task<IActionResult> Address(CertificateAddressViewModel vm)
        {
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmend/Address.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend", new { certificateId = vm.Id }), action: CertificateActions.Address);
        }
    }
}