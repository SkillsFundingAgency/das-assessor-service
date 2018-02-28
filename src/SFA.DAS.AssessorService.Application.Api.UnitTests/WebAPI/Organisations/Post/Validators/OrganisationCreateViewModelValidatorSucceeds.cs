﻿namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.UkPrnValidator
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
    public class WhenOrganisationCreateViewModelValidatorSucceeds : OrganisationCreateViewModelValidatorTestBase
    {
        private static ValidationResult _validationResult;
       

        Establish context = () =>
        {
            Setup();

            OrganisationCreateViewModel = Builder<CreateOrganisationRequest>.CreateNew()
                .With(q => q.EndPointAssessorOrganisationId = "1234")
                .With(q => q.EndPointAssessorName = "XXXX")
                .With(q => q.EndPointAssessorUKPRN = 10000001)
                .Build();

            ContactQueryRepositoryMock.Setup(q => q.CheckContactExists(Moq.It.IsAny<Guid>()))
                .Returns(Task.FromResult((true)));

            OrganisationQueryRepositoryMock.Setup(q => q.CheckIfAlreadyExists(Moq.It.IsAny<string>()))
                .Returns(Task.FromResult((false)));         
        };

        Because of = () =>
        {
            _validationResult = OrganisationCreateViewModelValidator.Validate(OrganisationCreateViewModel);
        };

        Machine.Specifications.It should_succeed = () =>
        {
            _validationResult.IsValid.Should().BeTrue();
        };

        Machine.Specifications.It errormessage_should_not_contain_EndPointAssessorOrganisationId = () =>
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorOrganisationId" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().BeNull();
        };

        Machine.Specifications.It errormessage_should_not_contain_EndPointAssessorName = () =>
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorName" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().BeNull();
        };

        Machine.Specifications.It errormessage_should_contain_EndPointAssessorUKPRN = () =>
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorUKPRN" && q.ErrorCode == "InclusiveBetweenValidator");
            errors.Should().BeNull();
        };

        Machine.Specifications.It errormessage_should_not_contain_PrimaryContactNotFound = () =>
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "PrimaryContactId" && q.ErrorCode == "PredicateValidator");
            errors.Should().BeNull();
        };

        Machine.Specifications.It errormessage_should_not_contain_AlreadyExists = () =>
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorOrganisationId" && q.ErrorCode == "PredicateValidator");
            errors.Should().BeNull();
        };
    }
}

