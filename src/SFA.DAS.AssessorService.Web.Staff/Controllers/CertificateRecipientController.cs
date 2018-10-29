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
    public class CertificateRecipientController : CertificateBaseController
    {
        public CertificateRecipientController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient)
            : base(logger, contextAccessor, apiClient)
        { }

        [HttpGet]
        public async Task<IActionResult> Recipient(Guid certificateid,
            string searchString,
            int page)
        {
            ViewBag.SearchString = searchString;
            ViewBag.Page = page;
            
            return await LoadViewModel<CertificateRecipientViewModel>(certificateid, "~/Views/CertificateAmend/Recipient.cshtml");
        }

        [HttpPost(Name = "Recipient")]
        public async Task<IActionResult> Recipient(CertificateRecipientViewModel vm,
            string searchString,
            int searchPage)                
        {
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmend/Recipient.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend",  new { certificateid = vm.Id, searchstring = searchString, page = searchPage }), action: CertificateActions.Recipient);
        }
    }
}