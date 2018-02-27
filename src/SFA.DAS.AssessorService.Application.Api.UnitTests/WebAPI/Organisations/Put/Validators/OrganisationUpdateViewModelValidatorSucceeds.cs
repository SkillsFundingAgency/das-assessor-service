namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.UkPrnValidator.Put
{
    using FluentValidation.Results;
    using Machine.Specifications;
    using SFA.DAS.AssessorService.Application.Api.Validators;
    using FluentAssertions;
    using FizzWare.NBuilder;
    using System.Linq;
    using System.Threading.Tasks;
    using System;
    using AssessorService.Api.Types.Models;

    [Subject("AssessorService")]
    public class WhenOrganisationUpdateViewModelValidatorSucceeds : OrganisationUpdateViewModelValidatorTestBase
    {
        private static ValidationResult _validationResult;
       
        Establish context = () =>
        {
            Setup();

            OrganisationUpdateViewModel = Builder<UpdateOrganisationRequest>.CreateNew()                
                .With(q => q.EndPointAssessorName = "Jane")   
                .With(q => q.PrimaryContactId = Guid.Empty)
                .Build();

            OrganisationQueryRepositoryMock.Setup(q => q.CheckIfAlreadyExists(Moq.It.IsAny<Guid>()))
                .Returns(Task.FromResult((true)));         
        };

        Because of = () =>
        {
            _validationResult = OrganisationUpdateViewModelValidator.Validate(OrganisationUpdateViewModel);
        };

        Machine.Specifications.It should_succeed = () =>
        {
            _validationResult.IsValid.Should().BeTrue();
        };

        Machine.Specifications.It errormessage_should_not_contain_EndPointAssessorName = () =>
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorName" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().BeNull();
        };

        Machine.Specifications.It errormessage_should_not_contain_DoesNotExist = () =>
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "Id" && q.ErrorCode == "PredicateValidator");
            errors.Should().BeNull();
        };
    }
}

