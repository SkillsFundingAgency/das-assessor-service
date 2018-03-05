using Moq;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.UnitTests.Helpers;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Organisations.Maintenence.Put.Validators
{
    public class OrganisationUpdateViewModelValidatorTestBase
    {
        protected static OrganisationUpdateViewModelValidator OrganisationUpdateViewModelValidator;
        protected static Mock<IContactQueryRepository> ContactQueryRepositoryMock;
        protected static Mock<IOrganisationQueryRepository> OrganisationQueryRepositoryMock;
        protected static UpdateOrganisationRequest OrganisationUpdateViewModel;        

        public static void Setup()
        {

            var mockStringLocaliserBuilder = new MockStringLocaliserBuilder();

            var stringLocalizerMock = mockStringLocaliserBuilder
                .WithKey(ResourceMessageName.NoAssesmentProviderFound)
                .WithKeyValue("100000000")
                .Build<OrganisationUpdateViewModelValidator>();

            ContactQueryRepositoryMock = new Mock<IContactQueryRepository>();
            OrganisationQueryRepositoryMock = new Mock<IOrganisationQueryRepository>();

            OrganisationUpdateViewModelValidator = new OrganisationUpdateViewModelValidator(stringLocalizerMock.Object,
                ContactQueryRepositoryMock.Object, OrganisationQueryRepositoryMock.Object);
        }
    }
}
