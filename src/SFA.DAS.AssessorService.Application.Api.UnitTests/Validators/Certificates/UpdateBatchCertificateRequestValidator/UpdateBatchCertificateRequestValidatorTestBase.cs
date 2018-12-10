using Microsoft.Extensions.Localization;
using Moq;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Certificates.UpdateBatchCertificateRequestValidator
{
    public class UpdateBatchCertificateRequestValidatorTestBase : BatchCertificateRequestValidatorTestBase
    {
        protected Api.Validators.Certificates.UpdateBatchCertificateRequestValidator Validator;

        public UpdateBatchCertificateRequestValidatorTestBase() : base()
        {
            var stringLocalizerMock = new Mock<IStringLocalizer<Api.Validators.Certificates.BatchCertificateRequestValidator>>();

            Validator = new Api.Validators.Certificates.UpdateBatchCertificateRequestValidator(stringLocalizerMock.Object, OrganisationQueryRepositoryMock.Object, IlrRepositoryMock.Object, CertificateRepositoryMock.Object, AssessmentOrgsApiClientMock.Object);
        }
    }
}
