using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.ViewModels.Search;

namespace SFA.DAS.AssessorService.Web.Controllers.Private
{
    [Authorize]
    [Route("certificate/privatedeclaration")]
    public class CertificatePrivateDeclarationController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOrganisationsApiClient _organisationsApiClient;

        public CertificatePrivateDeclarationController(
            IOrganisationsApiClient organisationsApiClient,
            ILogger<CertificateController> logger,
            IHttpContextAccessor contextAccessor)
        {
            _organisationsApiClient = organisationsApiClient;
            _contextAccessor = contextAccessor;
        }

        [HttpGet]
        public IActionResult Index(SearchRequestViewModel searchRequestViewModel)
        {           
            var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")?.Value;
            var organisation = _organisationsApiClient.Get(ukprn);
            // get certificate by uln and ukprn and last name
            //then check if certificate is submitted ...
//            if(certificate.Status == CertificateStatus.Submitted ||
//               certificate.Status == CertificateStatus.Printed ||
//               certificate.Status == CertificateStatus.Reprint ||
//               certificate.Status == CertificateStatus.Approved ||
//               certificate.Status == CertificateStatus.ToBeApproved ||
//               certificate.Status == CertificateStatus.Rejected ||
//               certificate.Status == CertificateStatus.SentForApproval)
//            
//            {
//                return RedirectToAction("Index", "CertificateAlreadySubmitted", new { id = certificate.Id } );                    
//            }
//            else
//            {
                return View("~/Views/Certificate/PrivateDeclaration.cshtml", searchRequestViewModel);
//            }
        }                
    }
}