using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Contacts.Maintenence.Post.Validators
{
    public class WhenCreateContactRequestValidatorFailsOnInvalidFieldLengths : CreateContactRequestValidatorTestBase
    {
        private static ValidationResult _validationResult;

        [SetUp]
        public void Arrange()
        {
            Setup();

            ContactRequest = Builder<CreateContactRequest>
                .CreateNew()
                .With(q => q.Username = q.Username.PadLeft(40, 'x'))
                .With(q => q.DisplayName =  q.DisplayName.PadLeft(140, 'x'))
                .With(q => q.Email = q.Email.PadLeft(140, 'x'))
                .Build();

            ContactQueryRepositoryMock.Setup(q => q.CheckContactExists(Moq.It.IsAny<string>()))
                .Returns(Task.FromResult((false)));

            OrganisationQueryRepositoryMock.Setup(q => q.CheckIfAlreadyExists(Moq.It.IsAny<string>()))
                .Returns(Task.FromResult((true)));


            _validationResult = CreateContactRequestValidator.Validate(ContactRequest);
        }

        [Test]
        public void ThenItShouldFail()
        {
            _validationResult.IsValid.Should().BeFalse();
        }

        [Test]
        public void ThenErrorMessageShouldContainInvalidEmailLength()
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "Email" && q.ErrorCode == "MaximumLengthValidator");
            errors.Should().NotBeNull();
        }

        [Test]
        public void ThenErrorMessageShouldContainInvalidDisplayNameLength()
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "DisplayName" && q.ErrorCode == "MaximumLengthValidator");
            errors.Should().NotBeNull();
        }

        [Test]
        public void ThenErrorMessageShouldContainInvalidUserNameLength()
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "Username" && q.ErrorCode == "MaximumLengthValidator");
            errors.Should().NotBeNull();
        }
    }
}

