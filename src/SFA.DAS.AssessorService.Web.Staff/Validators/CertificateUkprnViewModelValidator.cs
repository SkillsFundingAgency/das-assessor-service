using System;
using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.ExternalApis;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Private;

namespace SFA.DAS.AssessorService.Web.Staff.Validators
{
    public class CertificateUkprnViewModelValidator : AbstractValidator<CertificateUkprnViewModel>
    {
        private readonly IAssessmentOrgsApiClient _apiClient;

        public CertificateUkprnViewModelValidator(
            IAssessmentOrgsApiClient apiClient)
        {
            _apiClient = apiClient;
            RuleFor(vm => vm.Ukprn).NotEmpty()
                .WithMessage("Enter the training provider's UKPRN").DependentRules(() =>
            {
                RuleFor(vm => vm.Ukprn).Must(BeANumber).WithMessage("The UKPRN should contain 8 numbers").DependentRules(() =>
                {
                    RuleFor(vm => vm.Ukprn).Length(8).WithMessage("The UKPRN should contain 8 numbers").DependentRules(() =>
                    {
                        RuleFor(vm => vm.Ukprn).Must(UkprnMustExist).WithMessage("This UKPRN cannot be found");
                    });
                });
            });
        }

        private bool BeANumber(string number)
        {
            return int.TryParse(number, out int _);
        }

        private bool UkprnMustExist(string ukprn)
        {
            try
            {
                var providerUkprn = Convert.ToInt32(ukprn);
               _apiClient.GetProvider(providerUkprn).GetAwaiter().GetResult();
            }
            catch (EntityNotFoundException)
            {
                return false;
            }            

            return true;
        }
    }
}
