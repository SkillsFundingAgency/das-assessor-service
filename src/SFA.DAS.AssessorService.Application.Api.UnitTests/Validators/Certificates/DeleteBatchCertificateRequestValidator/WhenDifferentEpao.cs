using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Certificates.DeleteBatchCertificateRequestValidator
{
    public class WhenDifferentEpao : DeleteBatchCertificateRequestValidatorTestBase
    {
        private ValidationResult _validationResult;

        [SetUp]
        public async Task Arrange()
        {
            DeleteBatchCertificateRequest request = Builder<DeleteBatchCertificateRequest>.CreateNew()
                .With(i => i.Uln = 1234567890)
                .With(i => i.StandardCode = 1)
                .With(i => i.StandardReference = null)
                .With(i => i.UkPrn = 99999999)
                .With(i => i.FamilyName = "Test")
                .With(i => i.CertificateReference = "1234567890-1")
                .Build();

            _validationResult = await Validator.ValidateAsync(request);
        }

        [Test]
        public void ThenValidationResultShouldBeFalse()
        {
            _validationResult.IsValid.Should().BeFalse();
        }
    }
}
