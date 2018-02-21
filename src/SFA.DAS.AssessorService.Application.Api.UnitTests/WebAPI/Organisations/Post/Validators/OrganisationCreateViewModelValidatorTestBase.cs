namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.UkPrnValidator
{
    using Microsoft.Extensions.Localization;
    using Moq;
    using SFA.DAS.AssessorService.Application.Api.Consts;
    using SFA.DAS.AssessorService.Application.Api.Validators;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.ViewModel.Models;

    public class OrganisationCreateViewModelValidatorTestBase
    {
        protected static OrganisationCreateViewModelValidator OrganisationCreateViewModelValidator;
        protected static Mock<IContactRepository> ContactRepositoryMock;
        protected static Mock<IOrganisationQueryRepository> OrganisationQueryRepositoryMock;
        protected static OrganisationCreateViewModel OrganisationCreateViewModel;

        public static void Setup()
        {
            var stringLocalizerMock = new Mock<IStringLocalizer<OrganisationCreateViewModelValidator>>();
            string key = ResourceMessageName.NoAssesmentProviderFound;
            var localizedString = new LocalizedString(key, "10000000");
            stringLocalizerMock.Setup(q => q[Moq.It.IsAny<string>(), Moq.It.IsAny<string>()]).Returns(localizedString);

            ContactRepositoryMock = new Mock<IContactRepository>();
            OrganisationQueryRepositoryMock = new Mock<IOrganisationQueryRepository>();

            OrganisationCreateViewModelValidator = new OrganisationCreateViewModelValidator(stringLocalizerMock.Object,
                ContactRepositoryMock.Object, OrganisationQueryRepositoryMock.Object);
        }
    }
}
