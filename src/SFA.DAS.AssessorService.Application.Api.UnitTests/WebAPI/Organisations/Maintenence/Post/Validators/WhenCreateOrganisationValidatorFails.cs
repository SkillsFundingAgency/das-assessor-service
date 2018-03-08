using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Organisations.Maintenence.Post.Validators
{ 
    public class WhenCreateOrganisationRequestModelValidatorFails : OrganisationCreateRequestValidatorTestBase
    {
        private ValidationResult _validationResult;

        [SetUp]
        public void Arrange()
        {
            Setup();

            CreateOrganisationRequest = Builder<CreateOrganisationRequest>.CreateNew()
                .With(q => q.EndPointAssessorOrganisationId = null)
                .With(q => q.EndPointAssessorName = null)
                .With(q => q.EndPointAssessorUkprn = 12)
                .With(q => q.PrimaryContact = "1234")
                .Build();

            ContactQueryRepositoryMock.Setup(q => q.CheckContactExists(Moq.It.IsAny<string>()))
                .Returns(Task.FromResult((false)));

            OrganisationQueryRepositoryMock.Setup(q => q.CheckIfAlreadyExists(Moq.It.IsAny<string>()))
                .Returns(Task.FromResult((true)));

            _validationResult = CreateOrganisationRequestValidator.Validate(CreateOrganisationRequest);
        }

        [Test]
        public void ThenItShouldFail()
        {        
            _validationResult.IsValid.Should().BeFalse();
        }

        [Test]
        public void ErrorMessageShouldContain_EndPointAssessorOrganisation()       
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorOrganisationId" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().NotBeNull();
        }

        [Test]
        public void ErrorMessageShouldContainEndPointAssessorNam()
        {            
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorName" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().NotBeNull();
        }

        [Test]
        public void ErrorMessageShouldContainEndPointAssessorUkprn()    
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorUkprn" && q.ErrorCode == "InclusiveBetweenValidator");
            errors.Should().NotBeNull();
        }

        [Test]
        public void ErrorMessageShouldContainPrimaryContactNotFound()      
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "PrimaryContact" && q.ErrorCode == "PredicateValidator");
            errors.Should().NotBeNull();
        }

        [Test]
        public void ErrorMessageShouldContainAlreadyExists()       
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorOrganisationId" && q.ErrorCode == "PredicateValidator");
            errors.Should().NotBeNull();
        }
    }
}

