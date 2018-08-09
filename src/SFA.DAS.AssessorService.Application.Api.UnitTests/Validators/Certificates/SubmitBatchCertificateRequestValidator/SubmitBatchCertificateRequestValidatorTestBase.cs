using FizzWare.NBuilder;
using Microsoft.Extensions.Localization;
using Moq;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Certificates.SubmitBatchCertificateRequestValidator
{
    public class SubmitBatchCertificateRequestValidatorTestBase
    {
        private static Mock<ICertificateRepository> _CertificateRepositoryMock;

        protected static Api.Validators.Certificates.SubmitBatchCertificateRequestValidator Validator;

        protected static void Setup()
        {
            var stringLocalizerMock = SetupStringLocaliserMock();
            _CertificateRepositoryMock = SetupCertificateRepositoryMock();

            Validator = new Api.Validators.Certificates.SubmitBatchCertificateRequestValidator(stringLocalizerMock.Object, _CertificateRepositoryMock.Object);
        }

        private static Mock<IStringLocalizer<Api.Validators.Certificates.BatchCertificateRequestValidator>> SetupStringLocaliserMock()
        {
            var stringLocalizerMock = new Mock<IStringLocalizer<Api.Validators.Certificates.BatchCertificateRequestValidator>>();
            return stringLocalizerMock;
        }

        private static Mock<ICertificateRepository> SetupCertificateRepositoryMock()
        {
            var certificateRepositoryMock = new Mock<ICertificateRepository>();
            return certificateRepositoryMock;
        }

        protected static void AddMockCertificate(long uln, int standardCode, string certificateReference, string status)
        {
            var certificate = Builder<Certificate>.CreateNew()
                .With(i => i.Uln = uln)
                .With(i => i.StandardCode = standardCode)
                .With(i => i.CertificateReference = certificateReference)
                .With(i => i.Status = status)
                .Build();

            _CertificateRepositoryMock.Setup(q => q.GetCertificate(uln, standardCode)).ReturnsAsync(certificate);
        }
    }
}
