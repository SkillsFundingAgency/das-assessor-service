using System;
using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private;
using SFA.DAS.AssessorService.ExternalApis;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateUkprnViewModelValidator : AbstractValidator<CertificateUkprnViewModel>
    {
        private readonly IAssessmentOrgsApiClient _apiClient;

        public CertificateUkprnViewModelValidator(IStringLocalizer<CertificateUkprnViewModelValidator> localizer,
            IAssessmentOrgsApiClient apiClient)
        {
            _apiClient = apiClient;
            RuleFor(vm => vm.Ukprn).NotEmpty()
                .WithMessage(localizer["NameCannotBeEmpty"]).DependentRules(() =>
            {
                RuleFor(vm => vm.Ukprn).Must(BeANumber).WithMessage(localizer["UkprnNumber"]).DependentRules(() =>
                {
                    RuleFor(vm => vm.Ukprn).Length(8).WithMessage(localizer["UkprnNumberLength"]).DependentRules(() =>
                    {
                        RuleFor(vm => vm.Ukprn).Must(UkprnMustExist).WithMessage(localizer["UkprnMustExist"]);
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
            catch (EntityNotFoundException e)
            {
                return false;
            }            

            return true;
        }
    }
}
