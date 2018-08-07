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
        public CertificateApprenticeDetailsController(ILogger<CertificateAmmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient) : base(logger, contextAccessor, apiClient)
        {

        }

        [HttpGet]
        public async Task<IActionResult> ApprenticeDetail(Guid certificateid)
        {
            return await LoadViewModel<CertificateApprenticeDetailsViewModel>(certificateid, "~/Views/CertificateAmmend/ApprenticeDetail.cshtml");
        }
        
        [HttpPost(Name = "Date")]
        public async Task<IActionResult> ApprenticeDetail(CertificateApprenticeDetailsViewModel vm)
        {            
            var actionResult = await SaveViewModel(vm, 
                returnToIfModelNotValid: "~/Views/CertificateAmmend/ApprenticeDetail.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmmend", new { certificateid = vm.Id }), action: CertificateActions.Name);


            return actionResult;
        }
    }
}