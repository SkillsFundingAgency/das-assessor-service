using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using System.Threading.Tasks;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Route("certificate/familyname")]
    public class CertificateFamilyNameController : CertificateBaseController
    {
        private readonly IValidator<CertificateNamesViewModel> _certificateNameChangeValidator;
        public CertificateFamilyNameController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor, ICertificateApiClient certificateApiClient, ISessionService sessionService, IValidator<CertificateNamesViewModel> certificateNameChangeValidator) : base(logger, contextAccessor, certificateApiClient, sessionService)
        {
            _certificateNameChangeValidator = certificateNameChangeValidator;
        }

        [HttpGet]
        public async Task<IActionResult> FamilyName()
        {
            return await LoadViewModel<CertificateNamesViewModel>("~/Views/Certificate/FamilyName.cshtml", false);
        }

        [HttpPost]
        public async Task<IActionResult> FamilyName(CertificateNamesViewModel viewModel)
        {
            var validationResult = _certificateNameChangeValidator.Validate(viewModel);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View("/Views/Certificate/FamilyName.cshtml", viewModel);
            }

            viewModel.FamilyName = viewModel.InputFamilyName;

            return await SaveViewModel(viewModel,
                returnToIfModelNotValid: "~/Views/Certificate/FamilyName.cshtml",
                nextAction: RedirectToAction("Check", "CertificateCheck"), action: CertificateActions.LastName);
        }
    }
}
