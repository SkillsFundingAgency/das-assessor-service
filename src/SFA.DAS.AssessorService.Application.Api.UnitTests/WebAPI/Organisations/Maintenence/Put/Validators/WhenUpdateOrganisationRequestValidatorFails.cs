using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Organisations.Maintenence.Put.Validators
{
    public class WhenUpdateOrganisayionRequestValidatorFails : WhenUpdateOrganisationRequestValidatorTestBase
    {
        private static ValidationResult _validationResult;

        [SetUp]
        public void Arrange()
        { 
            Setup();

            UpdateOrganisationRequest = Builder<UpdateOrganisationRequest>.CreateNew()                
                .With(q => q.EndPointAssessorName = null)                                 
                .Build();

            ContactQueryRepositoryMock.Setup(q => q.CheckContactExists(Moq.It.IsAny<string>()))
                .Returns(Task.FromResult((false)));

            OrganisationQueryRepositoryMock.Setup(q => q.CheckIfAlreadyExists(Moq.It.IsAny<string>()))
                .Returns(Task.FromResult((false)));

            _validationResult = UpdateOrganisationRequestValidator.Validate(UpdateOrganisationRequest);
        }

        [Test]
        public void ThenItShouldFail()
        {                 
            _validationResult.IsValid.Should().BeFalse();
        }

        [Test]
        public void ErrorMessageShouldContainEndPointAssessorName()       
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorName" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().NotBeNull();
        }

        [Test]
        public void ErrormessageShouldContainPrimaryContactNotFound()                        
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "PrimaryContact" && q.ErrorCode == "PredicateValidator");
            errors.Should().NotBeNull();
        }

        [Test]
        public void ErrorMessageShouldContainDoesNotExist()       
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorOrganisationId" && q.ErrorCode == "PredicateValidator");
            errors.Should().NotBeNull();
        }
    }
}

