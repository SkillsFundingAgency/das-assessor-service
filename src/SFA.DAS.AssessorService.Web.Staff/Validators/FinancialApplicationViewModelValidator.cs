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
                    context.AddFailure("Grade.InadequateMoreInformation", "Enter why the application was graded inadequate");
                }
                else if (vm.Grade.SelectedGrade == FinancialApplicationSelectedGrade.Satisfactory && string.IsNullOrWhiteSpace(vm.Grade.SatisfactoryMoreInformation))
                {
                    context.AddFailure("Grade.SatisfactoryMoreInformation", "Enter why the application was graded satisfactory");
                }
                else if (string.IsNullOrWhiteSpace(vm.Grade.SelectedGrade))
                {
                    context.AddFailure("Grade.SelectedGrade", "Select a grade for this application");
                }
            });
        }
    }
}