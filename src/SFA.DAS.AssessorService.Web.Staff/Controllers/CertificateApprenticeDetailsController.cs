using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    public class CertificateApprenticeDetailsController : CertificateBaseController
    {      
        public CertificateApprenticeDetailsController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient) : base(logger, contextAccessor, apiClient)
        {

        }

        [HttpGet]
        public async Task<IActionResult> ApprenticeDetail(Guid certificateId)
        {
            return await LoadViewModel<CertificateApprenticeDetailsViewModel>(certificateId, "~/Views/CertificateAmend/ApprenticeDetail.cshtml");
        }
        
        [HttpPost(Name = "ApprenticeDetail")]
        public async Task<IActionResult> ApprenticeDetail(CertificateApprenticeDetailsViewModel vm)
        {            
            var actionResult = await SaveViewModel(vm, 
                returnToIfModelNotValid: "~/Views/CertificateAmend/ApprenticeDetail.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend", new { certificateId = vm.Id }), action: CertificateActions.Name);


            return actionResult;
        }
    }
}