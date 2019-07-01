using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.ExternalApi.Epas.UpdateBatchEpaRequestValidator
{
    public class WhenValidatorValidatesSuccessfully : UpdateBatchEpaRequestValidatorTestBase
    {
        private ValidationResult _validationResult;

        [SetUp]
        public async Task Arrange()
        {
            var epas = Builder<EpaRecord>.CreateListOfSize(1).All()
                .With(i => i.EpaDate = DateTime.UtcNow.AddDays(-1))
                .With(i => i.EpaOutcome = "pass")
                .Build().ToList();

            var request = Builder<UpdateBatchEpaRequest>.CreateNew()
                .With(i => i.Uln = 1234567890)
                .With(i => i.StandardCode = 1)
                .With(i => i.StandardReference = null)
                .With(i => i.UkPrn = 12345678)
                .With(i => i.FamilyName = "Test")
                .With(i => i.EpaDetails = new EpaDetails { Epas = epas, EpaReference = "1234567890-1" })
                .Build();

            _validationResult = await Validator.ValidateAsync(request);
        }

        [Test]
        public void ThenValidationResultShouldBeTrue()
        {
            _validationResult.IsValid.Should().BeTrue();
        }
    }
}
