using Moq;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.UnitTests.Helpers;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Organisations.Maintenence.Put.Validators
{
    public class WhenUpdateOrganisationRequestValidatorTestBase
    {
        protected static UpdateOrganisationRequestValidator UpdateOrganisationRequestValidator;
        protected static Mock<IContactQueryRepository> ContactQueryRepositoryMock;
        protected static Mock<IOrganisationQueryRepository> OrganisationQueryRepositoryMock;
        protected static UpdateOrganisationRequest UpdateOrganisationRequest;        

        public static void Setup()
        {

            var mockStringLocaliserBuilder = new MockStringLocaliserBuilder();

            var stringLocalizerMock = mockStringLocaliserBuilder
                .WithKey(ResourceMessageName.NoAssesmentProviderFound)
                .WithKeyValue("100000000")
                .Build<UpdateOrganisationRequestValidator>();

            ContactQueryRepositoryMock = new Mock<IContactQueryRepository>();
            OrganisationQueryRepositoryMock = new Mock<IOrganisationQueryRepository>();

            UpdateOrganisationRequestValidator = new UpdateOrganisationRequestValidator(stringLocalizerMock.Object,
                ContactQueryRepositoryMock.Object, OrganisationQueryRepositoryMock.Object);
        }
    }
}
