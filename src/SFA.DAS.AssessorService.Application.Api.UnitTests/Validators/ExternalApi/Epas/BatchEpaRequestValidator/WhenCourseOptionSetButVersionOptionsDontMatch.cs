using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.ExternalApi.Epas.BatchEpaRequestValidator
{
    public class WhenCourseOptionSetButVersionHasNoOptions : BatchEpaRequestValidatorTestBase
    {
        private ValidationResult _validationResult;

        [SetUp]
        public async Task Arrange()
        {
            var epas = Builder<EpaRecord>.CreateListOfSize(1).All()
                .With(i => i.EpaDate = DateTime.UtcNow.AddDays(-1))
                .With(i => i.EpaOutcome = EpaOutcome.Pass)
                .Build().ToList();

            var request = Builder<BatchEpaRequest>.CreateNew()
                .With(i => i.Uln = 1234567890)
                .With(i => i.StandardCode = 98)
                .With(i => i.StandardReference = "98")
                .With(i => i.UkPrn = 12345678)
                .With(i => i.FamilyName = "Test")
                .With(i => i.EpaDetails = new EpaDetails { Epas = epas })
                .With(i => i.Version = "1.0")
                .With(i => i.StandardUId = "ST98_1.0")
                .With(i => i.CourseOption = "CourseOption")
                .Build();

            _validationResult = await Validator.ValidateAsync(request);
        }

        [Test]
        public void ThenValidationResultShouldBeFalse()
        {
            _validationResult.IsValid.Should().BeFalse();
            _validationResult.Errors.Count.Should().Be(1);
            _validationResult.Errors.First().ErrorMessage.Should().Be("No course option available for this Standard and version. Must be empty");
        }
    }
}