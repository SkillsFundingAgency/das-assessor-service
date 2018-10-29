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
    [Route("certificate/lastname")]
    public class CertificatePrivateLastNameController : CertificateBaseController
    {
        public CertificatePrivateLastNameController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient) : base(logger, contextAccessor, apiClient)
        { }

        [HttpGet]
        public async Task<IActionResult> LastName(Guid certificateid,
            string searchString,
            int page)
        {
            ViewBag.SearchString = searchString;
            ViewBag.Page = page;
            
            return await LoadViewModel<CertificateLastNameViewModel>(certificateid, "~/Views/CertificateAmend/LastName.cshtml");
        }

        [HttpPost(Name = "LastName")]
        public async Task<IActionResult> LastName(CertificateLastNameViewModel vm,
            string searchString,
            int searchPage)
        {
            var actionResult = await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmend/LastName.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend",  new { certificateid = vm.Id, searchstring = searchString, page = searchPage }), action: CertificateActions.LastName);

            return actionResult;
        }
    }
}