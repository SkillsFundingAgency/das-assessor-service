using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [Route("certificate/declaration")]
    public class CertificateDeclarationController : CertificateBaseController
    {
        public CertificateDeclarationController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient, ISessionService sessionService) : base(logger, contextAccessor, certificateApiClient, sessionService)
        {}

        [HttpGet]
        public async Task<IActionResult> Declare()
        {
            return await LoadViewModel<CertificateDeclarationViewModel>("~/Views/Certificate/Declaration.cshtml");
        }
        
        [HttpPost(Name = "Declare")]
        public async Task<IActionResult> Declare(CertificateDeclarationViewModel vm)
        {
            return await SaveViewModel(vm, 
                returnToIfModelNotValid: "~/Views/Certificate/Declaration.cshtml",
                nextAction: RedirectToAction("Grade", "CertificateGrade"), action: CertificateActions.Declaration);
        }
    }
}