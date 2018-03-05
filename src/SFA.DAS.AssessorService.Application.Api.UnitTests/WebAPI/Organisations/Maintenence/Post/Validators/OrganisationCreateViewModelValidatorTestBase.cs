using Microsoft.Extensions.Localization;
using Moq;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.UnitTests.Helpers;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Organisations.Maintenence.Post.Validators
{
    public class OrganisationCreateViewModelValidatorTestBase
    {
        protected OrganisationCreateViewModelValidator OrganisationCreateViewModelValidator;
        protected Mock<IContactQueryRepository> ContactQueryRepositoryMock;
        protected Mock<IOrganisationQueryRepository> OrganisationQueryRepositoryMock;
        protected CreateOrganisationRequest OrganisationCreateViewModel;

        private Mock<IStringLocalizer<OrganisationCreateViewModelValidator>> organisationCreateViewModelValidatorStringLocaliser;

        private MockStringLocaliserBuilder _mockStringLocaliserBuilder;

        public void Setup()
        {
            _mockStringLocaliserBuilder = new MockStringLocaliserBuilder();

            organisationCreateViewModelValidatorStringLocaliser = _mockStringLocaliserBuilder
                .WithKey(ResourceMessageName.NoAssesmentProviderFound)
                .WithKeyValue("100000000")
                .Build<OrganisationCreateViewModelValidator>();

            ContactQueryRepositoryMock = new Mock<IContactQueryRepository>();
            OrganisationQueryRepositoryMock = new Mock<IOrganisationQueryRepository>();

            OrganisationCreateViewModelValidator = new OrganisationCreateViewModelValidator(organisationCreateViewModelValidatorStringLocaliser.Object,
                ContactQueryRepositoryMock.Object, OrganisationQueryRepositoryMock.Object);
        }
    }
}
