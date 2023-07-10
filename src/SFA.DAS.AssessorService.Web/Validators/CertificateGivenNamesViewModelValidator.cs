using FluentValidation.Results;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using System.Threading.Tasks;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateGivenNamesViewModelValidator
    {
        private readonly ICertificateApiClient _certificateApiClient;
        public CertificateGivenNamesViewModelValidator(ICertificateApiClient certificateApiClient)
        {
            _certificateApiClient = certificateApiClient;
        }

        public async Task<ValidationResult> Validate(CertificateGivenNamesViewModel viewModel)
        {
            var certificate = await _certificateApiClient.GetCertificate(viewModel.Id);
            var certData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);

            var originalGivenNames = certData.LearnerGivenNames;
            var validationResult = new ValidationResult();

            if (viewModel.GivenNames == string.Empty || viewModel.GivenNames == null)
            {
                validationResult.Errors.Add(new ValidationFailure("GivenNames", "Enter the apprentice's first name"));
            }
            else if (viewModel.GivenNames.ToLower() != originalGivenNames.ToLower())
            {
                validationResult.Errors.Add(new ValidationFailure("GivenNames", "Enter the apprentice's first name without changing the original spelling"));
            }

            return validationResult;
        }

    }
}
