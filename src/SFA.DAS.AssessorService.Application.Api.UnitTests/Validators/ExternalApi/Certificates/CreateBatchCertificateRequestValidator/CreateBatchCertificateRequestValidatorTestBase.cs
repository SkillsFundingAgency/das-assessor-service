using Microsoft.Extensions.Localization;
using Moq;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.ExternalApi.Certificates.CreateBatchCertificateRequestValidator
{
    public class CreateBatchCertificateRequestValidatorTestBase : BatchCertificateRequestValidatorTestBase
    {
        protected Api.Validators.ExternalApi.Certificates.CreateBatchCertificateRequestValidator Validator;

        public CreateBatchCertificateRequestValidatorTestBase() : base()
        {
            var stringLocalizerMock = new Mock<IStringLocalizer<Api.Validators.ExternalApi.Certificates.BatchCertificateRequestValidator>>();

            Validator = new Api.Validators.ExternalApi.Certificates.CreateBatchCertificateRequestValidator(stringLocalizerMock.Object, OrganisationQueryRepositoryMock.Object, IlrRepositoryMock.Object, CertificateRepositoryMock.Object, StandardServiceMock.Object);
        }
    }
}
