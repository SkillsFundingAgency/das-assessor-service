using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Organisations.Maintenence.Put.Validators
{
    public class WhenUpdateOrganisationRequestValidatorSucceeds : WhenUpdateOrganisationRequestValidatorTestBase
    {
        private static ValidationResult _validationResult;

        [SetUp]
        public void Arrange()
        {
            Setup();

            OrganisationUpdateViewModel = Builder<UpdateOrganisationRequest>.CreateNew()                
                .With(q => q.EndPointAssessorName = "Jane")   
                .With(q => q.PrimaryContact = null)
                .Build();

            OrganisationQueryRepositoryMock.Setup(q => q.CheckIfAlreadyExists(Moq.It.IsAny<string>()))
                .Returns(Task.FromResult((true)));

            _validationResult = UpdateOrganisationRequestValidator.Validate(OrganisationUpdateViewModel);
        }

        [Test]
        public void ThenItShouldSucceed()
        {
            _validationResult.IsValid.Should().BeTrue();
        }

        [Test]
        public void ErrormessageShouldNotContainEndPointAssessorName()        
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorName" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().BeNull();
        }

        [Test]
        public void ErrorMessageShouldNotContainDoesNotExist()
        {       
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorOrganisationId" && q.ErrorCode == "PredicateValidator");
            errors.Should().BeNull();
        }
    }
}

