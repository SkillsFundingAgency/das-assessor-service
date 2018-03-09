using Microsoft.Extensions.Localization;
using Moq;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.UnitTests.Helpers;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Contacts.Maintenence.Post.Validators
{
    public class CreateContactRequestValidatorTestBase
    {
        protected CreateContactRequestValidator CreateContactRequestValidator;
        protected Mock<IContactQueryRepository> ContactQueryRepositoryMock;
        protected Mock<IOrganisationQueryRepository> OrganisationQueryRepositoryMock;

        protected CreateContactRequest ContactRequest;

        private Mock<IStringLocalizer<CreateContactRequestValidator>> _contactCreateRequestValidatorLocaliser;

        private MockStringLocaliserBuilder _mockStringLocaliserBuilder;

        public void Setup()
        {
            _mockStringLocaliserBuilder = new MockStringLocaliserBuilder();

            _contactCreateRequestValidatorLocaliser = _mockStringLocaliserBuilder
                .WithKey(ResourceMessageName.NoAssesmentProviderFound)
                .WithKeyValue("100000000")
                .Build<CreateContactRequestValidator>();

            ContactQueryRepositoryMock = new Mock<IContactQueryRepository>();
            OrganisationQueryRepositoryMock = new Mock<IOrganisationQueryRepository>();

            CreateContactRequestValidator = new CreateContactRequestValidator(
                _contactCreateRequestValidatorLocaliser.Object,
                OrganisationQueryRepositoryMock.Object,
                ContactQueryRepositoryMock.Object);
        }
    }
}
