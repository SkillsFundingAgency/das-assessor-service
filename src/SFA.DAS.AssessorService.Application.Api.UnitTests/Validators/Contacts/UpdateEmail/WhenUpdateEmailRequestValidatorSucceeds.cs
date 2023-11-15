using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Contacts.UpdateEmail
{
    public class WhenUpdateEmailRequestValidatorSucceeds : UpdateEmailRequestValidatorTestBase
    {
        private static ValidationResult _validationResult;
        private static UpdateEmailRequest _updateEmailRequest;

        [SetUp]
        public void Arrange()
        {
            Setup();

            _updateEmailRequest = Builder<UpdateEmailRequest>
                .CreateNew()
                .With(q => q.Email = "xxxx@someemail.com")
                .With(q => q.NewEmail = "yyyy@someemail.com")
                .Build();

            ContactQueryRepositoryMock.Setup(q => q.CheckContactExists(Moq.It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            _validationResult = UpdateEmailRequestValidator.Validate(_updateEmailRequest);
        }

        [Test]
        public void ThenItShouldSucceed()
        {
            _validationResult.IsValid.Should().BeTrue();
        }

        [Test]
        public void ErrorMessageShouldNotContainEmail()
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "Email" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().BeNull();
        }

        [Test]
        public void ErrorMessageShouldNotContainNewEmail()
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "NewEmail" && q.ErrorCode == "NotEmptyValidator");
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
