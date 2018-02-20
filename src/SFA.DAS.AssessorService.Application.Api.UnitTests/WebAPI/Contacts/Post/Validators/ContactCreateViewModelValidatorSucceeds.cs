namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.UkPrnValidator
{
    using FluentValidation.Results;
    using Machine.Specifications;
    using SFA.DAS.AssessorService.Application.Api.Validators;
    using FluentAssertions;
    using FizzWare.NBuilder;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using System.Linq;
    using System.Threading.Tasks;

    [Subject("AssessorService")]
    public class WhenContactCreateViewModelValidatorSuccceeds : ContactCreateViewModelValidatorTestBase
    {
        private static ValidationResult _validationResult;


        Establish context = () =>
        {
            Setup();

            ContactCreateViewModel = Builder<ContactCreateViewModel>.CreateNew()
                .With(q => q.EndPointAssessorUKPRN = 10000001)
                .Build();
        };

        Because of = () =>
        {
            _validationResult = ContactCreateViewModelValidator.Validate(ContactCreateViewModel);
        };

        Machine.Specifications.It should_fail = () =>
        {
            _validationResult.IsValid.Should().BeTrue();
        };

        Machine.Specifications.It errormessage_should_contain_EndPointAssessorUKPRN = () =>
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorUKPRN" && q.ErrorCode == "InclusiveBetweenValidator");
            errors.Should().BeNull();
        };
    }
}



