using Microsoft.Extensions.Localization;
using Moq;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.ExternalApi.Certificates.BatchCertificateRequestValidator
{
    public class BatchCertificateRequestValidatorTestBase : Certificates.BatchCertificateRequestValidatorTestBase
    {
        protected Api.Validators.ExternalApi.Certificates.BatchCertificateRequestValidator Validator;

        public BatchCertificateRequestValidatorTestBase() : base()
        {
            var stringLocalizerMock = new Mock<IStringLocalizer<Api.Validators.ExternalApi.Certificates.BatchCertificateRequestValidator>>();

            Validator = new Api.Validators.ExternalApi.Certificates.BatchCertificateRequestValidator(stringLocalizerMock.Object, OrganisationQueryRepositoryMock.Object, IlrRepositoryMock.Object, CertificateRepositoryMock.Object, StandardServiceMock.Object);
        }
    }
}
