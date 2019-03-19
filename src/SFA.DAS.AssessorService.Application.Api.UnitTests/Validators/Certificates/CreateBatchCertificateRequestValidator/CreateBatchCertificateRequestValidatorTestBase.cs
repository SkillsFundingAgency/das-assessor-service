using Microsoft.Extensions.Localization;
using Moq;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Certificates.CreateBatchCertificateRequestValidator
{
    public class CreateBatchCertificateRequestValidatorTestBase : BatchCertificateRequestValidatorTestBase
    {
        protected Api.Validators.Certificates.CreateBatchCertificateRequestValidator Validator;

        public CreateBatchCertificateRequestValidatorTestBase() : base()
        {
            var stringLocalizerMock = new Mock<IStringLocalizer<Api.Validators.Certificates.BatchCertificateRequestValidator>>();

            Validator = new Api.Validators.Certificates.CreateBatchCertificateRequestValidator(stringLocalizerMock.Object, OrganisationQueryRepositoryMock.Object, IlrRepositoryMock.Object, CertificateRepositoryMock.Object, AssessmentOrgsApiClientMock.Object, StandardServiceMock.Object);
        }
    }
}
