using System.Linq;
using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using Machine.Specifications;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Contacts.Maintenence.Put.Validators
{
    [Subject("AssessorService")]
    public class WhenContactCreateViewModelValidatorSuccceeds : WhenUpdateContactRequestValidatorTestBase
    {
        private static ValidationResult _validationResult;
        private static UpdateContactRequest _updateContactRequest;

        [SetUp]
        public void Arrange()
        {
            Setup();

            _updateContactRequest = Builder<UpdateContactRequest>.CreateNew().Build();

            _validationResult = UpdateContactRequestValidator.Validate(_updateContactRequest);
        }

        [Test]
        public void ThenItShouldSucceed()
        {
            _validationResult.IsValid.Should().BeTrue();
        }

        [Test]
        public void ErrormessageShouldNotContainEMail()
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "Email" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().BeNull();
        }

        [Test]
        public void ErrorMessageChouldNotContainDisplayName()
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "DisplayName" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().BeNull();
        }

        [Test]
        public void ErrorMessageShouldNotContainUserName()
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "UserName" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().BeNull();
        }
    }
}



