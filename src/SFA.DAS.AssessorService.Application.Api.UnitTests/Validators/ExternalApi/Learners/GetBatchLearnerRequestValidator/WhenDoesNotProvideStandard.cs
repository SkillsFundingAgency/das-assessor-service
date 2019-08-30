using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Learners;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.ExternalApi.Learners.GetBatchLearnerRequestValidator
{
    public class WhenDoesNotProvideStandard : GetBatchLearnerRequestValidatorTestBase
    {
        private ValidationResult _validationResult;

        [SetUp]
        public async Task Arrange()
        {
            var request = Builder<GetBatchLearnerRequest>.CreateNew()
                .With(i => i.Uln = 1234567890)
                .With(i => i.Standard = "98")
                .With(i => i.UkPrn = 99999999)
                .With(i => i.FamilyName = "Test")
                .Build();

            _validationResult = await Validator.ValidateAsync(request);
        }

        [Test]
        public void ThenValidationResultShouldBeFalse()
        {
            _validationResult.IsValid.Should().BeFalse();
            _validationResult.Errors.Count.Should().Be(1);
        }
    }
}
