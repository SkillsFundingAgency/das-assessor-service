using Microsoft.Extensions.Localization;
using Moq;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.ExternalApi.Learners.GetBatchLearnerRequestValidator
{
    public class GetBatchLearnerRequestValidatorTestBase : ExternalApiValidatorsTestBase
    {
        protected Api.Validators.ExternalApi.Learners.GetBatchLearnerRequestValidator Validator;

        public GetBatchLearnerRequestValidatorTestBase() : base()
        {
            var stringLocalizerMock = new Mock<IStringLocalizer<Api.Validators.ExternalApi.Learners.GetBatchLearnerRequestValidator>>();
            Validator = new Api.Validators.ExternalApi.Learners.GetBatchLearnerRequestValidator(stringLocalizerMock.Object, OrganisationQueryRepositoryMock.Object, IlrRepositoryMock.Object, CertificateRepositoryMock.Object, StandardServiceMock.Object);
        }
    }
}
