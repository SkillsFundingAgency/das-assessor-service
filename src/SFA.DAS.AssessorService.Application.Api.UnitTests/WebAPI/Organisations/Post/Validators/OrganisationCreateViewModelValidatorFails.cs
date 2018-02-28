﻿namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.UkPrnValidator
{
    using FluentValidation.Results;
    using Machine.Specifications;
    using SFA.DAS.AssessorService.Application.Api.Validators;
    using FluentAssertions;
    using FizzWare.NBuilder;
    using System.Linq;
    using System.Threading.Tasks;
    using AssessorService.Api.Types.Models;

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
                .With(q => q.EndPointAssessorUKPRN = 12)
                .Build();

            ContactQueryRepositoryMock.Setup(q => q.CheckContactExists(Moq.It.IsAny<int>()))
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
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorUKPRN" && q.ErrorCode == "InclusiveBetweenValidator");
            errors.Should().NotBeNull();
        };

        Machine.Specifications.It errormessage_should_contain_PrimaryContactNotFound = () =>
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "PrimaryContactId" && q.ErrorCode == "PredicateValidator");
            errors.Should().NotBeNull();
        };

        Machine.Specifications.It errormessage_should_contain_AlreadyExists = () =>
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorOrganisationId" && q.ErrorCode == "PredicateValidator");
            errors.Should().NotBeNull();
        };
    }
}

