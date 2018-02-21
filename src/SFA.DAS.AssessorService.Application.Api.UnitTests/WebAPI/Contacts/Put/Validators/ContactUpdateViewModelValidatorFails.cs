namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.ContactContoller.Put.Validators
{
    using FluentValidation.Results;
    using Machine.Specifications;
    using SFA.DAS.AssessorService.Application.Api.Validators;
    using FluentAssertions;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using System.Linq;

    [Subject("AssessorService")]
    public class ContactUpdateViewModelValidatorFails : ContactUpdateViewModelValidatorTestBase
    {
        private static ValidationResult _validationResult;
        private static ContactUpdateViewModel _contactUpdateViewModel;

        Establish context = () =>
        {
            Setup();

            _contactUpdateViewModel = new ContactUpdateViewModel();
        };

        Because of = () =>
        {
            _validationResult = ContactUpdateViewModelValidator.Validate(_contactUpdateViewModel);
        };

        Machine.Specifications.It should_fail = () =>
        {
            _validationResult.IsValid.Should().BeFalse();
        };
        
        Machine.Specifications.It errormessage_should_contain_EndPointAssessorUKPRN = () =>
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "ContactEmail" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().NotBeNull();
        };

        Machine.Specifications.It errormessage_should_contain_ContactName = () =>
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "ContactName" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().NotBeNull();
        };      
    }
}

