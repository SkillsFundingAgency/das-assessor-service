using FizzWare.NBuilder;
using Microsoft.Extensions.Localization;
using Moq;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Certificates.Batch
{
    public class BatchCertificateRequestValidatorBase
    {
        protected const long Uln_Certificate = 1234;
        protected const int StandardCode_Certificate = 1234;

        protected static Api.Validators.BatchCertificateRequestValidator BatchCertificateRequestValidator;

        protected static void Setup()
        {
            var stringLocalizerMock = SetupStringLocaliserMock();
            var certificateRepositoryMock = SetupCertificateRepositoryMock(Uln_Certificate, StandardCode_Certificate);

            BatchCertificateRequestValidator = new Api.Validators.BatchCertificateRequestValidator(stringLocalizerMock.Object, certificateRepositoryMock.Object);
        }

        private static Mock<IStringLocalizer<Api.Validators.BatchCertificateRequestValidator>> SetupStringLocaliserMock()
        {
            var stringLocalizerMock = new Mock<IStringLocalizer<Api.Validators.BatchCertificateRequestValidator>>();
            return stringLocalizerMock;
        }

        private static Mock<ICertificateRepository> SetupCertificateRepositoryMock(long uln, int standardCode)
        {
            var certificate = Builder<Certificate>.CreateNew().With(i => i.Uln = uln).With(i => i.StandardCode = standardCode).Build();

            var certificateRepositoryMock = new Mock<ICertificateRepository>();

            certificateRepositoryMock.Setup(q => q.GetCertificate(uln, standardCode)).ReturnsAsync(certificate);

            return certificateRepositoryMock;
        }
    }
}
