using Microsoft.Extensions.Localization;
using Moq;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.ExternalApi.Certificates.GetBatchCertificateRequestValidator
{
    public class GetBatchCertificateRequestValidatorTestBase : BatchCertificateRequestValidatorTestBase
    {
        protected Api.Validators.ExternalApi.Certificates.GetBatchCertificateRequestValidator Validator;

        public GetBatchCertificateRequestValidatorTestBase() : base()
        {
            var stringLocalizerMock = new Mock<IStringLocalizer<Api.Validators.ExternalApi.Certificates.GetBatchCertificateRequestValidator>>();
            Validator = new Api.Validators.ExternalApi.Certificates.GetBatchCertificateRequestValidator(stringLocalizerMock.Object, OrganisationQueryRepositoryMock.Object, IlrRepositoryMock.Object, CertificateRepositoryMock.Object, StandardServiceMock.Object);
        }
    }
}
