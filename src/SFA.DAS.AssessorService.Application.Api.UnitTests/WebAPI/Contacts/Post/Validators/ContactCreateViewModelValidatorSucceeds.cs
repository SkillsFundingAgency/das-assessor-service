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

            ContactCreateViewModel = Builder<ContactCreateViewModel>.CreateNew().Build();            
        };

        Because of = () =>
        {
            _validationResult = ContactCreateViewModelValidator.Validate(ContactCreateViewModel);
        };

        Machine.Specifications.It should_fail = () =>
        {
            _validationResult.IsValid.Should().BeTrue();
        };

        Machine.Specifications.It errormessage_should_not_contain_EndPointAssessorUKPRN = () =>
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "ContactEmail" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().BeNull();
        };

        Machine.Specifications.It errormessage_should_not_contain_ContactName = () =>
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "ContactName" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().BeNull();
        };

        Machine.Specifications.It errormessage_should_not_contain_EndPointOrganisationId = () =>
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorContactId" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().BeNull();
        };
    }
}



