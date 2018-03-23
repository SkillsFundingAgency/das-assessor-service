using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Contacts.Update
{
    public class WhenUpdateContactRequestValidatorSuccceeds : UpdateContactRequestValidatorTestBase
    {
        private static ValidationResult _validationResult;
        private static UpdateContactRequest _updateContactRequest;

        [SetUp]
        public void Arrange()
        {
            Setup();

            _updateContactRequest = Builder<UpdateContactRequest>
                .CreateNew()
                .With(q => q.Email = "james@gmail.com")
                .Build();

            ContactQueryRepositoryMock.Setup(q => q.CheckContactExists(Moq.It.IsAny<string>()))
                .Returns(Task.FromResult(true));
                
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



