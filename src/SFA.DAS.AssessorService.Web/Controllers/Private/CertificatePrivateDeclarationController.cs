using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.ViewModels.Search;

namespace SFA.DAS.AssessorService.Web.Controllers.Private
{
    [Authorize]
    [Route("certificate/privatedeclaration")]
    public class CertificatePrivateDeclarationController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOrganisationsApiClient _organisationsApiClient;
        private readonly ICertificateApiClient _certificateApiClient;

        public CertificatePrivateDeclarationController(
            IOrganisationsApiClient organisationsApiClient,
            ICertificateApiClient certificateApiClient,
            ILogger<CertificateController> logger,
            IHttpContextAccessor contextAccessor)
        {
            _organisationsApiClient = organisationsApiClient;
            _certificateApiClient = certificateApiClient;
            _contextAccessor = contextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> Index(SearchRequestViewModel searchRequestViewModel)
        {
            var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")?.Value;
            var organisation = await _organisationsApiClient.Get(ukprn);

            var certificate = await _certificateApiClient.GetCertificate(Convert.ToInt32(ukprn),
                Convert.ToInt64(searchRequestViewModel.Uln),
                searchRequestViewModel.Surname,
                organisation.EndPointAssessorOrganisationId);
            
            if(certificate.Status == CertificateStatus.Submitted ||
               certificate.Status == CertificateStatus.Printed ||
               certificate.Status == CertificateStatus.Reprint ||
               certificate.Status == CertificateStatus.Approved ||
               certificate.Status == CertificateStatus.ToBeApproved ||
               certificate.Status == CertificateStatus.Rejected ||
               certificate.Status == CertificateStatus.SentForApproval)
            
            {
                return RedirectToAction("Index", "CertificateAlreadySubmitted", new { id = certificate.Id } );                    
            }

            return View("~/Views/Certificate/PrivateDeclaration.cshtml", searchRequestViewModel);
        }
    }
}