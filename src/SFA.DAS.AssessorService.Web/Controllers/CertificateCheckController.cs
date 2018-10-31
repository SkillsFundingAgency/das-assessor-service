using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [Route("certificate/check")]
    public class CertificateCheckController : CertificateBaseController
    {
        private IOrganisationsApiClient _organisationsApiClient;
        
        public CertificateCheckController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient, 
            IOrganisationsApiClient organisationsApiClient,
            ISessionService sessionService) : base(logger, contextAccessor,
            certificateApiClient, sessionService)
        {
            _organisationsApiClient = organisationsApiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Check()
        {
            var sessionString = SessionService.Get("CertificateSession");
            if (sessionString == null)
            {
                return RedirectToAction("Index", "Search");
            }

            var certSession = JsonConvert.DeserializeObject<CertificateSession>(sessionString);
            TempData["HideOption"] = !certSession.Options.Any();

            return await LoadViewModel<CertificateCheckViewModel>("~/Views/Certificate/Check.cshtml");
        }

        [HttpGet("CheckForRejectedApprovals/{certificateId}")]
        public async Task<IActionResult> CheckForRejectedApprovals(Guid certificateId)
        {
            SessionService.Remove("SearchResults");
            SessionService.Remove("SelectedStandard");
            SessionService.Remove("SearchResultsChooseStandard");
            SessionService.Remove("EndPointAsessorOrganisationId");
            
            var viewModel = new CertificateCheckViewModel();
            var certificate = await CertificateApiClient.GetCertificate(certificateId);
            viewModel.FromCertificate(certificate);

            var organisation = await _organisationsApiClient.Get(certificate.OrganisationId);

            viewModel.BackToSearchPage = true;
            
            SessionService.Set("CertificateSession", new CertificateSession
            {
                CertificateId = certificate.Id,
                Uln = certificate.Uln,
                StandardCode = certificate.StandardCode               
            });

            SessionService.Set("EndPointAsessorOrganisationId", organisation.EndPointAssessorOrganisationId);

            TempData["HideOption"] = false;
            
            return View("~/Views/Certificate/Check.cshtml", viewModel);
        }

        [HttpPost(Name = "Check")]
        public async Task<IActionResult> Check(CertificateCheckViewModel vm)
        {
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/Certificate/Check.cshtml",
                nextAction: RedirectToAction("Confirm", "CertificateConfirmation"), action: CertificateActions.Submit);
        }
    }
}