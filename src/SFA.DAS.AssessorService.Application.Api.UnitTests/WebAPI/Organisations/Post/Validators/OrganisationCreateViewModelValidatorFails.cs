namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Organisations.Post.Validators
{
    using System.Linq;
    using System.Threading.Tasks;
    using AssessorService.Api.Types.Models;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentValidation.Results;
    using Machine.Specifications;
    using UnitTests.Validators.UkPrnValidator;

    [Subject("AssessorService")]
    public class WhenOrganisationCreateViewModelValidatorFails : OrganisationCreateViewModelValidatorTestBase
    {
        private static ValidationResult _validationResult;
       

        Establish context = () =>
        {
            Setup();

            OrganisationCreateViewModel = Builder<CreateOrganisationRequest>.CreateNew()
                .With(q => q.EndPointAssessorOrganisationId = null)
                .With(q => q.EndPointAssessorName = null)
                .With(q => q.EndPointAssessorUkprn = 12)
                .With(q => q.PrimaryContact = "1234")
                .Build();

            ContactQueryRepositoryMock.Setup(q => q.CheckContactExists(Moq.It.IsAny<string>()))
                .Returns(Task.FromResult((false)));

            OrganisationQueryRepositoryMock.Setup(q => q.CheckIfAlreadyExists(Moq.It.IsAny<string>()))
                .Returns(Task.FromResult((true)));         
        };

        Because of = () =>
        {
            _validationResult = OrganisationCreateViewModelValidator.Validate(OrganisationCreateViewModel);
        };

        Machine.Specifications.It should_fail = () =>
        {
            _validationResult.IsValid.Should().BeFalse();
        };

        Machine.Specifications.It errormessage_should_contain_EndPointAssessorOrganisationId = () =>
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorOrganisationId" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().NotBeNull();
        };

        Machine.Specifications.It errormessage_should_contain_EndPointAssessorName = () =>
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorName" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().NotBeNull();
        };

        Machine.Specifications.It errormessage_should_contain_EndPointAssessorUKPRN = () =>
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorUkprn" && q.ErrorCode == "InclusiveBetweenValidator");
            errors.Should().NotBeNull();
        };

        Machine.Specifications.It errormessage_should_contain_PrimaryContactNotFound = () =>
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "PrimaryContact" && q.ErrorCode == "PredicateValidator");
            errors.Should().NotBeNull();
        };

        Machine.Specifications.It errormessage_should_contain_AlreadyExists = () =>
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorOrganisationId" && q.ErrorCode == "PredicateValidator");
            errors.Should().NotBeNull();
        };
    }
}

