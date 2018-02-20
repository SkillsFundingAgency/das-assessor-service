namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.UkPrnValidator
{
    using Microsoft.Extensions.Localization;
    using Moq;
    using SFA.DAS.AssessorService.Application.Api.Consts;
    using SFA.DAS.AssessorService.Application.Api.Validators;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.ViewModel.Models;

    public class OrganisationUpdateViewModelValidatorTestBase
    {
        protected static OrganisationUpdateViewModelValidator OrganisationUpdateViewModelValidator;
        protected static Mock<IContactRepository> ContactRepositoryMock;
        protected static Mock<IOrganisationRepository> OrganisationRepositoryMock;
        protected static OrganisationUpdateViewModel OrganisationUpdateViewModel;

        public static void Setup()
        {
            var stringLocalizerMock = new Mock<IStringLocalizer<OrganisationUpdateViewModelValidator>>();
            string key = ResourceMessageName.NoAssesmentProviderFound;
            var localizedString = new LocalizedString(key, "10000000");
            stringLocalizerMock.Setup(q => q[Moq.It.IsAny<string>(), Moq.It.IsAny<string>()]).Returns(localizedString);

            ContactRepositoryMock = new Mock<IContactRepository>();
            OrganisationRepositoryMock = new Mock<IOrganisationRepository>();

            OrganisationUpdateViewModelValidator = new OrganisationUpdateViewModelValidator(stringLocalizerMock.Object,
                ContactRepositoryMock.Object, OrganisationRepositoryMock.Object);
        }
    }
}
