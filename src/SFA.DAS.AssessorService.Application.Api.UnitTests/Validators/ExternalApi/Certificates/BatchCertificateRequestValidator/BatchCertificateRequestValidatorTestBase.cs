using Microsoft.Extensions.Localization;
using Moq;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.ExternalApi.Certificates.BatchCertificateRequestValidator
{
    public class BatchCertificateRequestValidatorTestBase : ExternalApiValidatorsTestBase
    {
        protected Api.Validators.ExternalApi.Certificates.BatchCertificateRequestValidator Validator;

        public BatchCertificateRequestValidatorTestBase() : base()
        {
            var stringLocalizerMock = new Mock<IStringLocalizer<Api.Validators.ExternalApi.Certificates.BatchCertificateRequestValidator>>();

            Validator = new Api.Validators.ExternalApi.Certificates.BatchCertificateRequestValidator(stringLocalizerMock.Object, OrganisationQueryRepositoryMock.Object, LearnerRepositoryMock.Object, StandardServiceMock.Object);
        }
    }
}
