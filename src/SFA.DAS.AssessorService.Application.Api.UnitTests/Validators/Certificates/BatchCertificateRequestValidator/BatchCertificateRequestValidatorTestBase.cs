using Microsoft.Extensions.Localization;
using Moq;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Certificates.BatchCertificateRequestValidator
{
    public class BatchCertificateRequestValidatorTestBase : Certificates.BatchCertificateRequestValidatorTestBase
    {
        protected Api.Validators.Certificates.BatchCertificateRequestValidator Validator;

        public BatchCertificateRequestValidatorTestBase() : base()
        {
            var stringLocalizerMock = new Mock<IStringLocalizer<Api.Validators.Certificates.BatchCertificateRequestValidator>>();

            Validator = new Api.Validators.Certificates.BatchCertificateRequestValidator(stringLocalizerMock.Object, OrganisationQueryRepositoryMock.Object, IlrRepositoryMock.Object, CertificateRepositoryMock.Object, AssessmentOrgsApiClientMock.Object, StandardServiceMock.Object);
        }
    }
}
