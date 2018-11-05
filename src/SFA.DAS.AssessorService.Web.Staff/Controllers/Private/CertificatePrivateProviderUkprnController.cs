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
    [Route("certificate/ukprns")]
    public class CertificatePrivateProviderUkprnController : CertificateBaseController
    { 
        public CertificatePrivateProviderUkprnController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient) : base(logger, contextAccessor, apiClient)
        {     
        }

        [HttpGet]
        public async Task<IActionResult> Ukprn(Guid certificateId)
        {
            var viewResult = await LoadViewModel<CertificateUkprnViewModel>(certificateId, "~/Views/CertificateAmend/Ukprn.cshtml");
            return viewResult;
        }

        [HttpPost(Name = "Ukprn")]
        public async Task<IActionResult> Ukprn(CertificateUkprnViewModel vm)
        {
            var actionResult = await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmend/Ukprn.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend", new { certificateId = vm.Id }), action: CertificateActions.ProviderUkPrn);

            return actionResult;           
        }
    }
}