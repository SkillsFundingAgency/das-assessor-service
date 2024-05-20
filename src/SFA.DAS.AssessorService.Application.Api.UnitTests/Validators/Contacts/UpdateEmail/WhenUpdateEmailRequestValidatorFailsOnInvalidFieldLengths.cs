using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Contacts.UpdateEmail
{
    public class WhenUpdateEmailRequestValidatorFailsOnInvalidFieldLengths : UpdateEmailRequestValidatorTestBase
    {
        private static ValidationResult _validationResult;

        [SetUp]
        public void Arrange()
        {
            Setup();

            var contactRequest = Builder<UpdateEmailRequest>
                .CreateNew()
                .With(q => q.GovUkIdentifier = q.GovUkIdentifier.PadLeft(10, 'x'))
                .With(q => q.NewEmail = q.NewEmail.PadLeft(257, 'x'))
                .Build();

            ContactQueryRepositoryMock.Setup(q => q.GetContactFromGovIdentifier(Moq.It.IsAny<string>()))
                .ReturnsAsync(new Contact());

            OrganisationQueryRepositoryMock.Setup(q => q.CheckIfAlreadyExists(Moq.It.IsAny<string>()))
                .Returns(Task.FromResult(true));


            _validationResult = UpdateEmailRequestValidator.Validate(contactRequest);
        }

        [Test]
        public void ThenItShouldFail()
        {
            _validationResult.IsValid.Should().BeFalse();
        }

        [Test]
        public void ThenErrorMessageShouldContainInvalidFieldLength()
        {
            _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "NewEmail").Should().NotBeNull();
            _validationResult.Errors.Count.Should().Be(2);
        }
    }
}
