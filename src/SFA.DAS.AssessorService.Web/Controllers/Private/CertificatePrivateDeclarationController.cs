using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Web.ViewModels.Search;

namespace SFA.DAS.AssessorService.Web.Controllers.Private
{
    [Authorize]
    [Route("certificate/privatedeclaration")]
    public class CertificatePrivateDeclarationController : Controller
    {
        public CertificatePrivateDeclarationController(ILogger<CertificateController> logger) 
        {}

        [HttpGet]
        public IActionResult Index(SearchRequestViewModel searchRequestViewModel)
        {           
            return View("~/Views/Certificate/PrivateDeclaration.cshtml", searchRequestViewModel);            
        }                
    }
}