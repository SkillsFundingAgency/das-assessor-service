using System.Linq;
using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Contacts.Maintenence.Post.Validators
{
    public class WhenCreateContactRequestValidatorSuccceeds : CreateContactRequestValidatorTestBase
    {
        private static ValidationResult _validationResult;

        [SetUp]
        public void Arrange()
        { 
            Setup();

            ContactRequest = Builder<CreateContactRequest>.CreateNew().Build();

            _validationResult = CreateContactRequestValidator.Validate(ContactRequest);
        }

        [Test]
        public void ThenItShouldSucceed()
        {
            _validationResult.IsValid.Should().BeTrue();
        }

        [Test]
        public void ErrorMessageShouldNotContainEndPointAssessorUkprn()
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "ContactEmail" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().BeNull();
        }

        [SetUp]
        public void ErrormessageShouldNotContainContactName()
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "ContactName" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().BeNull();
        }

        [SetUp]
        public void ErrorNessageShouldNotContainEndPointOrganisationId()
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorContactId" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().BeNull();
        }
    }
}



