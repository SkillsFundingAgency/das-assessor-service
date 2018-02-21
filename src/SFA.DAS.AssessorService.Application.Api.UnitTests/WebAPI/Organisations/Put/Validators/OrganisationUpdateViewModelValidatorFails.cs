namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.UkPrnValidator.Put
{
    using FluentValidation.Results;
    using Machine.Specifications;
    using SFA.DAS.AssessorService.Application.Api.Validators;
    using FluentAssertions;
    using FizzWare.NBuilder;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using System.Linq;
    using System.Threading.Tasks;
    using System;

    [Subject("AssessorService")]
    public class WhenOrganisationUpdateViewModelValidatorFails : OrganisationUpdateViewModelValidatorTestBase
    {
        private static ValidationResult _validationResult;
       

        Establish context = () =>
        {
            Setup();

            OrganisationUpdateViewModel = Builder<OrganisationUpdateViewModel>.CreateNew()                
                .With(q => q.EndPointAssessorName = null)                
                .With(q => q.PrimaryContactId = 999)                  
                .Build();

            ContactRepositoryMock.Setup(q => q.CheckContactExists(Moq.It.IsAny<int>()))
                .Returns(Task.FromResult((false)));

            OrganisationRepositoryMock.Setup(q => q.CheckIfAlreadyExists(Moq.It.IsAny<Guid>()))
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
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "Id" && q.ErrorCode == "PredicateValidator");
            errors.Should().NotBeNull();
        };
    }
}

