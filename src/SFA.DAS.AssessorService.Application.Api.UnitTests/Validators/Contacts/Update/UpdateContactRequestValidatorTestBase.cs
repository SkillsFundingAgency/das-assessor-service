using Moq;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.UnitTests.Helpers;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Contacts.Update
{
    public class UpdateContactRequestValidatorTestBase
    {
        protected static UpdateContactRequestValidator UpdateContactRequestValidator;
        protected static Mock<IContactQueryRepository> ContactQueryRepositoryMock;
        protected static Mock<IOrganisationQueryRepository> OrganisationQueryRepositoryMock;
        protected static UpdateOrganisationRequest UpdateOrganisationRequest;

        public static void Setup()
        {

            var mockStringLocaliserBuilder = new MockStringLocaliserBuilder();

            var stringLocalizerMock = mockStringLocaliserBuilder
                .WithKey(ResourceMessageName.NoAssesmentProviderFound)
                .WithKeyValue("100000000")
                .Build<UpdateContactRequestValidator>();

            ContactQueryRepositoryMock = new Mock<IContactQueryRepository>();
            OrganisationQueryRepositoryMock = new Mock<IOrganisationQueryRepository>();

            UpdateContactRequestValidator = new UpdateContactRequestValidator(stringLocalizerMock.Object,
                ContactQueryRepositoryMock.Object);
        }
    }
}
