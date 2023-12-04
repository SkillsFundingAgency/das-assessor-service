using Moq;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.UnitTests.Helpers;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Contacts.UpdateEmail
{
    public class UpdateEmailRequestValidatorTestBase
    {
        private protected static UpdateEmailRequestValidator UpdateEmailRequestValidator;
        private protected static Mock<IContactQueryRepository> ContactQueryRepositoryMock;
        private protected static Mock<IOrganisationQueryRepository> OrganisationQueryRepositoryMock;

        public static void Setup()
        {
            var mockStringLocaliserBuilder = new MockStringLocaliserBuilder();

            var stringLocalizerMock = mockStringLocaliserBuilder
                .WithKey(ResourceMessageName.NoAssesmentProviderFound)
                .WithKeyValue("100000000")
                .Build<UpdateEmailRequestValidator>();

            ContactQueryRepositoryMock = new Mock<IContactQueryRepository>();
            OrganisationQueryRepositoryMock = new Mock<IOrganisationQueryRepository>();

            UpdateEmailRequestValidator = new UpdateEmailRequestValidator(stringLocalizerMock.Object,
                ContactQueryRepositoryMock.Object);
        }
    }
}
