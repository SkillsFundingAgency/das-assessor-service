using Microsoft.Extensions.Localization;
using Moq;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.ExternalApi.Epas.UpdateBatchEpaRequestValidator
{
    public class UpdateBatchEpaRequestValidatorTestBase : ExternalApiValidatorsTestBase
    {
        protected Api.Validators.ExternalApi.Epas.UpdateBatchEpaRequestValidator Validator;

        public UpdateBatchEpaRequestValidatorTestBase() : base()
        {
            var stringLocalizerMock = new Mock<IStringLocalizer<Api.Validators.ExternalApi.Epas.BatchEpaRequestValidator>>();

            Validator = new Api.Validators.ExternalApi.Epas.UpdateBatchEpaRequestValidator(stringLocalizerMock.Object, OrganisationQueryRepositoryMock.Object, IlrRepositoryMock.Object, CertificateRepositoryMock.Object, StandardServiceMock.Object);
        }
    }
}
