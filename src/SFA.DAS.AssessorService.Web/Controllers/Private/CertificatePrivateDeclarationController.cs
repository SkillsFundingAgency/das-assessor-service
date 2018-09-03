using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [Route("certificate/privatedeclaration")]
    public class CertificatePrivateDeclarationController : Controller
    {
        public CertificatePrivateDeclarationController(ILogger<CertificateController> logger) 
        {}

        [HttpGet]
        public IActionResult Index()
        {
            CertificateDeclarationViewModel certificateDeclarationViewModel = new CertificateDeclarationViewModel();

            certificateDeclarationViewModel.IsPrivatelyFunded = true;

            return View("~/Views/Certificate/Declaration.cshtml", certificateDeclarationViewModel);            
        }                
    }
}