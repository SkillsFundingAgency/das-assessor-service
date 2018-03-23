using System.Linq;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Contacts.Update
{
    public class WhenUpdateViewModelValidatorFails : UpdateContactRequestValidatorTestBase
    {
        private static ValidationResult _validationResult;
        private static UpdateContactRequest _updateContactRequest;

        [SetUp]
        public void Arrange()
        {
            Setup();

            _updateContactRequest = new UpdateContactRequest();
            _validationResult = UpdateContactRequestValidator.Validate(_updateContactRequest);
        }

        [Test]
        public void ThenItShouldFail()
        {
            _validationResult.IsValid.Should().BeFalse();
        }

        [Test]
        public void ErrorMessageShouldContainUserName()
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "UserName" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().NotBeNull();
        }
    }
}

