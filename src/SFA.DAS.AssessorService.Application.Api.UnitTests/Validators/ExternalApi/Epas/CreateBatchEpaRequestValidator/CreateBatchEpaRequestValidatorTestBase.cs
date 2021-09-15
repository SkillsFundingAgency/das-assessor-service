using Microsoft.Extensions.Localization;
using Moq;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.ExternalApi.Epas.CreateBatchEpaRequestValidator
{
    public class CreateBatchEpaRequestValidatorTestBase : ExternalApiValidatorsTestBase
    {
        protected Api.Validators.ExternalApi.Epas.CreateBatchEpaRequestValidator Validator;

        public CreateBatchEpaRequestValidatorTestBase() : base()
        {
            var stringLocalizerMock = new Mock<IStringLocalizer<Api.Validators.ExternalApi.Epas.BatchEpaRequestValidator>>();

            Validator = new Api.Validators.ExternalApi.Epas.CreateBatchEpaRequestValidator(stringLocalizerMock.Object, OrganisationQueryRepositoryMock.Object, LearnerRepositoryMock.Object, CertificateRepositoryMock.Object, StandardServiceMock.Object);
        }
    }
}
