using System.Linq;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Contacts.UpdateEmail
{
    public class WhenUpdateEmailViewModelValidatorFails : UpdateEmailRequestValidatorTestBase
    {
        private static ValidationResult _validationResult;
        private static UpdateEmailRequest _updateEmailRequest;

        [SetUp]
        public void Arrange()
        {
            Setup();

            _updateEmailRequest = new UpdateEmailRequest();
            _validationResult = UpdateEmailRequestValidator.Validate(_updateEmailRequest);
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
