﻿namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.UkPrnValidator.Put
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
    public class WhenOrganisationUpdateViewModelValidatorFails : OrganisationUpdateViewModelValidatorTestBase
    {
        private static ValidationResult _validationResult;
       
        Establish context = () =>
        {
            Setup();

            OrganisationUpdateViewModel = Builder<UpdateOrganisationRequest>.CreateNew()                
                .With(q => q.EndPointAssessorName = null)                                 
                .Build();

            ContactQueryRepositoryMock.Setup(q => q.CheckContactExists(Moq.It.IsAny<int>()))
                .Returns(Task.FromResult((false)));

            OrganisationQueryRepositoryMock.Setup(q => q.CheckIfAlreadyExists(Moq.It.IsAny<string>()))
                .Returns(Task.FromResult((false)));         
        };

        Because of = () =>
        {
            _validationResult = OrganisationUpdateViewModelValidator.Validate(OrganisationUpdateViewModel);
        };

        Machine.Specifications.It should_fail = () =>
        {
            _validationResult.IsValid.Should().BeFalse();
        };

        Machine.Specifications.It errormessage_should_contain_EndPointAssessorName = () =>
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorName" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().NotBeNull();
        };

        
        Machine.Specifications.It errormessage_should_contain_PrimaryContactNotFound = () =>
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "PrimaryContactId" && q.ErrorCode == "PredicateValidator");
            errors.Should().NotBeNull();
        };

        Machine.Specifications.It errormessage_should_contain_DoesNotExist = () =>
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "EndPointAssessorOrganisationId" && q.ErrorCode == "PredicateValidator");
            errors.Should().NotBeNull();
        };
    }
}

