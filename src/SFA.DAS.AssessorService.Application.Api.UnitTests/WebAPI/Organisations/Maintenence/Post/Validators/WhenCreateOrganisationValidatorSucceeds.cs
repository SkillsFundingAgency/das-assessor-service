using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Organisations.Maintenence.Post.Validators
{ 
    public class WhenCreateOrganisationRequestValidatorSucceeds : OrganisationCreateRequestValidatorTestBase
    {
        private ValidationResult _validationResult;

        [SetUp]
        public void Arrange()
        { 
            Setup();

            CreateOrganisationRequest = Builder<CreateOrganisationRequest>.CreateNew()  
                .With(q => q.EndPointAssessorOrganisationId = "123456")
                .With(q => q.EndPointAssessorUkprn = 10000001)
                .With(q =>  q.PrimaryContact = null)
                .Build();

            ContactQueryRepositoryMock.Setup(q => q.CheckContactExists(Moq.It.IsAny<string>()))
                .Returns(Task.FromResult((true)));

            OrganisationQueryRepositoryMock.Setup(q => q.CheckIfAlreadyExists(Moq.It.IsAny<string>()))
                .Returns(Task.FromResult((false)));

            _validationResult = CreateOrganisationRequestValidator.Validate(CreateOrganisationRequest);
        }

        [Test]
        public void ThenItShouldSucceed()        
        {
            _validationResult.IsValid.Should().BeTrue();
        }

        [Test]
        public void ErrorMessageShouldNotContainEndPointAssessorOrganisationId()      
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorOrganisationId" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().BeNull();
        }

        [Test]
        public void ErrorMessageShouldNotMaxLengthErrorForEndPointAssessorOrganisationIdMustBeDefined()
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorOrganisationId" && q.ErrorCode == "NMaximumLengthValidator");
            errors.Should().BeNull();
        }

        [Test]
        public void ErrorMessageShouldNotContainEndPointAssessorName()    
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorName" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().BeNull();
        }

        [Test]
        public void ErrorMessageShouldNotContainEndPointAssessorUkPrn()       
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorUKPRN" && q.ErrorCode == "InclusiveBetweenValidator");
            errors.Should().BeNull();
        }

        [Test]
        public void ErrorMessageShouldNotContainPrimaryContactNotFound()       
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "PrimaryContact");
            errors.Should().BeNull();
        }

        [Test]
        public void ErrorMessageShouldNotContainAlreadyExists()        
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "Organisation");
            errors.Should().BeNull();
        }
    }
}

