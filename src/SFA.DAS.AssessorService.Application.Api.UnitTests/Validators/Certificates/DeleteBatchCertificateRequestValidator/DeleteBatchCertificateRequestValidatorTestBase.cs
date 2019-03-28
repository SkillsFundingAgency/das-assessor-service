using Microsoft.Extensions.Localization;
using Moq;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Certificates.DeleteBatchCertificateRequestValidator
{
    public class DeleteBatchCertificateRequestValidatorTestBase : BatchCertificateRequestValidatorTestBase
    {
        protected Api.Validators.Certificates.DeleteBatchCertificateRequestValidator Validator;

        public DeleteBatchCertificateRequestValidatorTestBase () : base()
        {
            var stringLocalizerMock = new Mock<IStringLocalizer<Api.Validators.Certificates.DeleteBatchCertificateRequestValidator>>();

            Validator = new Api.Validators.Certificates.DeleteBatchCertificateRequestValidator(stringLocalizerMock.Object, OrganisationQueryRepositoryMock.Object, IlrRepositoryMock.Object, CertificateRepositoryMock.Object, AssessmentOrgsApiClientMock.Object, StandardServiceMock.Object);
        }

    }
}
