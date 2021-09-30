using Microsoft.Extensions.Localization;
using Moq;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.ExternalApi.Epas.BatchEpaRequestValidator
{
    public class BatchEpaRequestValidatorTestBase : ExternalApiValidatorsTestBase
    {
        protected Api.Validators.ExternalApi.Epas.BatchEpaRequestValidator Validator;

        public BatchEpaRequestValidatorTestBase() : base()
        {
            var stringLocalizerMock = new Mock<IStringLocalizer<Api.Validators.ExternalApi.Epas.BatchEpaRequestValidator>>();

            Validator = new Api.Validators.ExternalApi.Epas.BatchEpaRequestValidator(stringLocalizerMock.Object, OrganisationQueryRepositoryMock.Object, LearnerRepositoryMock.Object, StandardServiceMock.Object);
        }
    }
}
