using FluentValidation.Results;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateFamilyNameViewModelValidator
    {
        private readonly ICertificateApiClient _certificateApiClient;
        public CertificateFamilyNameViewModelValidator(ICertificateApiClient certificateApiClient)
        {
            _certificateApiClient = certificateApiClient;
        }

        public async Task<ValidationResult> Validate(CertificateFamilyNameViewModel viewModel)
        {
            var certificate = await _certificateApiClient.GetCertificate(viewModel.Id);
            var certData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
            var originalFamilyName = certData.LearnerFamilyName;
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(viewModel.FamilyName))
            {
                validationResult.Errors.Add(new ValidationFailure("FamilyName", "Enter the apprentice's last name"));
            }
            else if (viewModel.FamilyName.ToLower() != originalFamilyName.ToLower())
            {
                validationResult.Errors.Add(new ValidationFailure("FamilyName", "Enter the apprentice's last name without changing the original spelling"));
            }

            return validationResult;
        }
    }
}
