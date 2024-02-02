using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Validators;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using System.Threading.Tasks;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Route("certificate/familyname")]
    public class CertificateFamilyNameController : CertificateBaseController
    {
        private readonly CertificateFamilyNameViewModelValidator _certificateNameChangeValidator;
        public CertificateFamilyNameController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor, ICertificateApiClient certificateApiClient, ISessionService sessionService, CertificateFamilyNameViewModelValidator certificateNameChangeValidator) : base(logger, contextAccessor, certificateApiClient, sessionService)
        {
            _certificateNameChangeValidator = certificateNameChangeValidator;
        }

        [HttpGet]
        public async Task<IActionResult> FamilyName()
        {
            return await LoadViewModel<CertificateFamilyNameViewModel>("~/Views/Certificate/FamilyName.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> FamilyName(CertificateFamilyNameViewModel viewModel)
        {
            var validationResult = await _certificateNameChangeValidator.Validate(viewModel);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View("/Views/Certificate/FamilyName.cshtml", viewModel);
            }

            return await SaveViewModel(viewModel,
                returnToIfModelNotValid: "~/Views/Certificate/FamilyName.cshtml",
                nextAction: RedirectToAction("Check", "CertificateCheck"), action: CertificateActions.FamilyName);
        }
    }
}
