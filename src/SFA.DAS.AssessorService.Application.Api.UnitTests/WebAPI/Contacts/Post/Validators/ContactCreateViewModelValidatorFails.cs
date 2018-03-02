namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.UkPrnValidator
{
    using FluentValidation.Results;
    using Machine.Specifications;
    using SFA.DAS.AssessorService.Application.Api.Validators;
    using FluentAssertions;
    using System.Linq;
    using AssessorService.Api.Types.Models;

    [Subject("AssessorService")]
    public class WhenContactCreateViewModelValidatorFails : ContactCreateViewModelValidatorTestBase
    {
        private static ValidationResult _validationResult;
       

        Establish context = () =>
        {
            Setup();

            ContactCreateViewModel = new CreateContactRequest();
        };

        Because of = () =>
        {
            _validationResult = ContactCreateViewModelValidator.Validate(ContactCreateViewModel);
        };

        Machine.Specifications.It should_fail = () =>
        {
            _validationResult.IsValid.Should().BeFalse();
        };
        
        Machine.Specifications.It errormessage_should_contain_email = () =>
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "Email" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().NotBeNull();
        };

        Machine.Specifications.It errormessage_should_contain_displayname = () =>
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "DisplayName" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().NotBeNull();
        };

        Machine.Specifications.It errormessage_should_contain_EndPointOrganisationId = () =>
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorOrganisationId" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().NotBeNull();
        };

        Machine.Specifications.It errormessage_should_contain_username = () =>
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "Username" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().NotBeNull();
        };
    }
}

