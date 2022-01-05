using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateCheckViewModelValidator : AbstractValidator<CertificateCheckViewModel>
    {
        public CertificateCheckViewModelValidator(IStringLocalizer<CertificateCheckViewModelValidator> localizer)
        {
            RuleFor(vm => vm.Version).NotEmpty()
                .WithMessage(localizer["VersionCannotBeNull"]);

            When(vm => vm.StandardHasOptions, () =>
            {
                RuleFor(vm => vm.Option).NotNull()
                    .WithMessage(localizer["OptionCannotBeNull"]);
            });

            RuleFor(vm => vm.SelectedGrade).NotNull()
                .WithMessage(localizer["GradeCannotBeNull"]);

            When(vm => vm.SelectedGrade != CertificateGrade.Fail, () =>
            {
                RuleFor(vm => vm.SendTo).NotEqual(CertificateSendTo.None)
                    .WithMessage(localizer["SendToCannotBeNone"]);

                RuleFor(vm => vm.Name).NotEmpty()
                    .WithMessage(localizer["NameCannotBeEmpty"]);

                When(vm => vm.SendTo == CertificateSendTo.Employer, () =>
                {
                    RuleFor(vm => vm.Employer).NotNull()
                        .DependentRules(() =>
                        {
                            RuleFor(vm => vm.AddressLine1).NotNull()
                                .DependentRules(() =>
                                {
                                    RuleFor(vm => vm.City).NotNull()
                                        .DependentRules(() =>
                                    {
                                        RuleFor(vm => vm.Postcode).NotNull();
                                    });
                                });
                        })
                        .WithMessage(localizer["AddressCannotBeEmpty"]); ;
                }).Otherwise(() => 
                {
                    RuleFor(vm => vm.AddressLine1).NotNull()
                        .DependentRules(() =>
                        {
                            RuleFor(vm => vm.City).NotNull()
                                .DependentRules(() =>
                                {
                                    RuleFor(vm => vm.Postcode).NotNull();
                                });
                        })
                        .WithMessage(localizer["AddressCannotBeEmpty"]); ;
                });

                When(vm => vm.SelectedGrade != null, () =>
                {
                    RuleFor(vm => vm.AchievementDate).NotNull()
                       .WithMessage(localizer["AchievementDateCannotBeEmpty"]);

                });
            });

            When(vm => vm.SelectedGrade == CertificateGrade.Fail, () =>
            {
                RuleFor(vm => vm.AchievementDate).NotNull()
                .WithMessage(localizer["FailDateCannotBeEmpty"]);
            });

            When(vm => vm.SelectedGrade == null, () =>
            {
                RuleFor(vm => vm.AchievementDate).NotNull()
                .WithMessage(localizer["DateCannotBeEmpty"]);
            });
        }
    }
}