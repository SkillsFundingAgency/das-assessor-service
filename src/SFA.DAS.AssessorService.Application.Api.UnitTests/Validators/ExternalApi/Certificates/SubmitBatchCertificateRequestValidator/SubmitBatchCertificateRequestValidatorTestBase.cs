using Microsoft.Extensions.Localization;
using Moq;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.ExternalApi.Certificates.SubmitBatchCertificateRequestValidator
{
    public class SubmitBatchCertificateRequestValidatorTestBase : ExternalApiValidatorsTestBase
    {
        protected Api.Validators.ExternalApi.Certificates.SubmitBatchCertificateRequestValidator Validator;

        public SubmitBatchCertificateRequestValidatorTestBase() : base()
        {
            var stringLocalizerMock = new Mock<IStringLocalizer<Api.Validators.ExternalApi.Certificates.SubmitBatchCertificateRequestValidator>>();
            Validator = new Api.Validators.ExternalApi.Certificates.SubmitBatchCertificateRequestValidator(stringLocalizerMock.Object, OrganisationQueryRepositoryMock.Object, LearnerRepositoryMock.Object, CertificateRepositoryMock.Object, StandardServiceMock.Object);
        }
    }
}
