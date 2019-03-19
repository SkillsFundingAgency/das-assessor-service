using Microsoft.Extensions.Localization;
using Moq;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Certificates.GetBatchCertificateRequestValidator
{
    public class GetBatchCertificateRequestValidatorTestBase : BatchCertificateRequestValidatorTestBase
    {
        protected Api.Validators.Certificates.GetBatchCertificateRequestValidator Validator;

        public GetBatchCertificateRequestValidatorTestBase() : base()
        {
            var stringLocalizerMock = new Mock<IStringLocalizer<Api.Validators.Certificates.GetBatchCertificateRequestValidator>>();
            Validator = new Api.Validators.Certificates.GetBatchCertificateRequestValidator(stringLocalizerMock.Object, OrganisationQueryRepositoryMock.Object, IlrRepositoryMock.Object, CertificateRepositoryMock.Object, AssessmentOrgsApiClientMock.Object, StandardServiceMock.Object);
        }
    }
}
