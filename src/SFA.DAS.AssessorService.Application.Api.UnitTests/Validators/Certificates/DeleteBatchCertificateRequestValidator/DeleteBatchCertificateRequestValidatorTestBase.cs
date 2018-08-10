using FizzWare.NBuilder;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Certificates.DeleteBatchCertificateRequestValidator
{
    public class DeleteBatchCertificateRequestValidatorTestBase
    {
        private static Mock<ICertificateRepository> _CertificateRepositoryMock;

        protected static Api.Validators.Certificates.DeleteBatchCertificateRequestValidator Validator;

        protected static void Setup()
        {
            var stringLocalizerMock = SetupStringLocaliserMock();
            _CertificateRepositoryMock = SetupCertificateRepositoryMock();

            Validator = new Api.Validators.Certificates.DeleteBatchCertificateRequestValidator(stringLocalizerMock.Object, _CertificateRepositoryMock.Object);
        }

        private static Mock<IStringLocalizer<Api.Validators.Certificates.DeleteBatchCertificateRequestValidator>> SetupStringLocaliserMock()
        {
            var stringLocalizerMock = new Mock<IStringLocalizer<Api.Validators.Certificates.DeleteBatchCertificateRequestValidator>>();
            return stringLocalizerMock;
        }

        private static Mock<ICertificateRepository> SetupCertificateRepositoryMock()
        {
            var certificateRepositoryMock = new Mock<ICertificateRepository>();
            return certificateRepositoryMock;
        }

        protected static void AddMockCertificate(long uln, int standardCode, string familyname, string status)
        {
            var certificate = Builder<Certificate>.CreateNew()
                .With(i => i.Uln = uln)
                .With(i => i.StandardCode = standardCode)
                .With(i => i.Status = status)
                .With(i => i.CertificateData = JsonConvert.SerializeObject(Builder<CertificateData>.CreateNew()
                                .With(cd => cd.LearnerFamilyName = familyname)
                                .Build()))
                .Build();

            _CertificateRepositoryMock.Setup(q => q.GetCertificate(uln, standardCode)).ReturnsAsync(certificate);
        }
    }
}
