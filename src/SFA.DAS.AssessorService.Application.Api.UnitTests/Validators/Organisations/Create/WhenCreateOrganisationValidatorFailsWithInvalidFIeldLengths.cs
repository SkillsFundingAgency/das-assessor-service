using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Organisations.Create
{ 
    public class WhenCreateOrganisationValidatorFailsWithInvalidFIeldLengths : OrganisationCreateRequestValidatorTestBase
    {
        private ValidationResult _validationResult;

        [SetUp]
        public void Arrange()
        {
            Setup();

            CreateOrganisationRequest = Builder<CreateOrganisationRequest>.CreateNew()
                .With(q => q.EndPointAssessorOrganisationId = "1234567890123456")
                .With(q => q.EndPointAssessorUkprn = 10000001)
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
        public void ErrorMessageShouldNotMaxLengthErrorForEndPointAssessorOrganisationIdMustBeDefined()
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorOrganisationId" && q.ErrorCode == "NMaximumLengthValidator");
            errors.Should().BeNull();
        }
    }
}

