using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Contacts.Update
{
    public class WhenUpdateContactRequestValidatorFailsOnInvalidFieldLengths : UpdateContactRequestValidatorTestBase
    {
        private static ValidationResult _validationResult;

        [SetUp]
        public void Arrange()
        {
            Setup();

            var contactRequest = Builder<UpdateContactRequest>
                .CreateNew()
                .With(q => q.UserName = q.UserName.PadLeft(40, 'x'))
                .With(q => q.DisplayName =  q.DisplayName.PadLeft(140, 'x'))
                .With(q => q.Email = q.Email.PadLeft(140, 'x'))
                .Build();

            ContactQueryRepositoryMock.Setup(q => q.CheckContactExists(Moq.It.IsAny<string>()))
                .Returns(Task.FromResult((false)));

            OrganisationQueryRepositoryMock.Setup(q => q.CheckIfAlreadyExists(Moq.It.IsAny<string>()))
                .Returns(Task.FromResult((true)));


            _validationResult = UpdateContactRequestValidator.Validate(contactRequest);
        }

        [Test]
        public void ThenItShouldFail()
        {
            _validationResult.IsValid.Should().BeFalse();
        }

        [Test]
        public void ThenErrorMessageShouldContainInvalidUserNameLength()
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "UserName" && q.ErrorCode == "MaximumLengthValidator");
            errors.Should().NotBeNull();
        }
    }
}

