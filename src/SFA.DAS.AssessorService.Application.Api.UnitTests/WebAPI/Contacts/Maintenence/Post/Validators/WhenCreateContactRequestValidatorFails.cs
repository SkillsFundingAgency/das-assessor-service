using System.Linq;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Contacts.Maintenence.Post.Validators
{
    public class WhenCreateContactRequestValidatorFails : CreateContactRequestValidatorTestBase
    {
        private static ValidationResult _validationResult;

        [SetUp]
        public void Arrange()
        {
            Setup();

            ContactRequest = new CreateContactRequest();

            _validationResult = CreateContactRequestValidator.Validate(ContactRequest);
        }

        [Test]
        public void ThenItShouldFail()
        {
            _validationResult.IsValid.Should().BeFalse();
        }

        [Test]
        public void ThenErrorMessageShouldContainEmail()
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "Email" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().NotBeNull();
        }

        [Test]
        public void ThenErrorMessageShouldContainDisplayName()
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "DisplayName" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().NotBeNull();
        }

        [Test]
        public void ThenErrorMessageShouldContainEndPointOrganisationId()
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorOrganisationId" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().NotBeNull();
        }

        [Test]
        public void ThenErrorMessageShouldContaUsername()
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "Username" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().NotBeNull();
        }
    }
}

