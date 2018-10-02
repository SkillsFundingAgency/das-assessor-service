using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<IActionResult> ApprenticeDetail(Guid certificateid)
        {
            return await LoadViewModel<CertificateApprenticeDetailsViewModel>(certificateid, "~/Views/CertificateAmend/ApprenticeDetail.cshtml");
        }
        
        [HttpPost(Name = "ApprenticeDetail")]
        public async Task<IActionResult> ApprenticeDetail(CertificateApprenticeDetailsViewModel vm)
        {            
            var actionResult = await SaveViewModel(vm, 
                returnToIfModelNotValid: "~/Views/CertificateAmend/ApprenticeDetail.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend", new { certificateid = vm.Id }), action: CertificateActions.Name);


            return actionResult;
        }
    }
}