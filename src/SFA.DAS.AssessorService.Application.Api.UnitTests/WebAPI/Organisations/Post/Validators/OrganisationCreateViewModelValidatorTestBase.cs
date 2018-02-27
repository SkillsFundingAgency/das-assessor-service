﻿namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.UkPrnValidator
{
    using AssessorService.Api.Types.Models;
    using Microsoft.Extensions.Localization;
    using Moq;
    using SFA.DAS.AssessorService.Application.Api.Consts;
    using SFA.DAS.AssessorService.Application.Api.Validators;
    using SFA.DAS.AssessorService.Application.Interfaces;

    public class OrganisationCreateViewModelValidatorTestBase
    {
        protected static OrganisationCreateViewModelValidator OrganisationCreateViewModelValidator;
        protected static Mock<IContactQueryRepository> ContactQueryRepositoryMock;
        protected static Mock<IOrganisationQueryRepository> OrganisationQueryRepositoryMock;
        protected static CreateOrganisationRequest OrganisationCreateViewModel;

        public static void Setup()
        {
            var stringLocalizerMock = new Mock<IStringLocalizer<OrganisationCreateViewModelValidator>>();
            string key = ResourceMessageName.NoAssesmentProviderFound;
            var localizedString = new LocalizedString(key, "10000000");
            stringLocalizerMock.Setup(q => q[Moq.It.IsAny<string>(), Moq.It.IsAny<string>()]).Returns(localizedString);

            ContactQueryRepositoryMock = new Mock<IContactQueryRepository>();
            OrganisationQueryRepositoryMock = new Mock<IOrganisationQueryRepository>();

            OrganisationCreateViewModelValidator = new OrganisationCreateViewModelValidator(stringLocalizerMock.Object,
                ContactQueryRepositoryMock.Object, OrganisationQueryRepositoryMock.Object);
        }
    }
}
