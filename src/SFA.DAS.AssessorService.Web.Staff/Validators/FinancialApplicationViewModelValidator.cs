using FluentValidation;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Web.Staff.Controllers.Apply;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Apply.Financial;

namespace SFA.DAS.AssessorService.Web.Staff.Validators
{
    public class FinancialApplicationViewModelValidator  : AbstractValidator<FinancialApplicationViewModel>
    {
        public FinancialApplicationViewModelValidator()
        {
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                if (vm.Grade.SelectedGrade == FinancialApplicationSelectedGrade.Inadequate && string.IsNullOrWhiteSpace(vm.Grade.InadequateMoreInformation))
                {
                    context.AddFailure("Grade.InadequateMoreInformation", "Please enter a value for Inadequate");
                }
                else if (vm.Grade.SelectedGrade == FinancialApplicationSelectedGrade.Satisfactory && string.IsNullOrWhiteSpace(vm.Grade.SatisfactoryMoreInformation))
                {
                    context.AddFailure("Grade.SatisfactoryMoreInformation", "Please enter a value for Satisfactory");
                }
                else if (string.IsNullOrWhiteSpace(vm.Grade.SelectedGrade))
                {
                    context.AddFailure("Grade.SelectedGrade", "Please select a grade");
                }
            });
        }
    }
}