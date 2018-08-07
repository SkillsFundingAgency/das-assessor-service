using FizzWare.NBuilder;
using Microsoft.Extensions.Localization;
using Moq;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Certificates.BatchCertificateRequestValidator
{
    public class BatchCertificateRequestValidatorTestBase
    {
        protected static Api.Validators.Certificates.BatchCertificateRequestValidator Validator;

        protected static void Setup()
        {
            var stringLocalizerMock = SetupStringLocaliserMock();
            Validator = new Api.Validators.Certificates.BatchCertificateRequestValidator(stringLocalizerMock.Object);
        }

        private static Mock<IStringLocalizer<Api.Validators.Certificates.BatchCertificateRequestValidator>> SetupStringLocaliserMock()
        {
            var stringLocalizerMock = new Mock<IStringLocalizer<Api.Validators.Certificates.BatchCertificateRequestValidator>>();
            return stringLocalizerMock;
        }
    }
}
