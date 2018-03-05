namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.ContactContoller.Put.Validators
{
    using FluentValidation.Results;
    using Machine.Specifications;
    using SFA.DAS.AssessorService.Application.Api.Validators;
    using FluentAssertions;
    using FizzWare.NBuilder;
    using System.Linq;
    using AssessorService.Api.Types.Models;

    [Subject("AssessorService")]
    public class WhenContactCreateViewModelValidatorSuccceeds : ContactUpdateViewModelValidatorTestBase
    {
        private static ValidationResult _validationResult;
        private static UpdateContactRequest _contactUpdateViewModel;

        Establish context = () =>
        {
            Setup();

            _contactUpdateViewModel = Builder<UpdateContactRequest>.CreateNew().Build();            
        };

        Because of = () =>
        {
            _validationResult = ContactUpdateViewModelValidator.Validate(_contactUpdateViewModel);
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
    }
}



