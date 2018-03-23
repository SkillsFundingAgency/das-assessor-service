using Microsoft.Extensions.Localization;
using Moq;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.UnitTests.Helpers;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Organisations
{
    public class OrganisationCreateRequestValidatorTestBase
    {
        protected CreateOrganisationRequestValidator CreateOrganisationRequestValidator;
        protected Mock<IContactQueryRepository> ContactQueryRepositoryMock;
        protected Mock<IOrganisationQueryRepository> OrganisationQueryRepositoryMock;
        protected CreateOrganisationRequest CreateOrganisationRequest;

        private Mock<IStringLocalizer<CreateOrganisationRequestValidator>> createOrganisationRequestModelValidatorStringLocaliser;

        private MockStringLocaliserBuilder _mockStringLocaliserBuilder;

        public void Setup()
        {
            _mockStringLocaliserBuilder = new MockStringLocaliserBuilder();

            createOrganisationRequestModelValidatorStringLocaliser = _mockStringLocaliserBuilder
                .WithKey(ResourceMessageName.NoAssesmentProviderFound)
                .WithKeyValue("100000000")
                .Build<CreateOrganisationRequestValidator>();

            ContactQueryRepositoryMock = new Mock<IContactQueryRepository>();
            OrganisationQueryRepositoryMock = new Mock<IOrganisationQueryRepository>();

            CreateOrganisationRequestValidator = new CreateOrganisationRequestValidator(createOrganisationRequestModelValidatorStringLocaliser.Object,
                ContactQueryRepositoryMock.Object, OrganisationQueryRepositoryMock.Object);
        }
    }
}
