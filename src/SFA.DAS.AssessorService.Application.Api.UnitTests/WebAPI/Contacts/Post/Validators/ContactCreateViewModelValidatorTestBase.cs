namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.UkPrnValidator
{
    using Microsoft.Extensions.Localization;
    using Moq;
    using SFA.DAS.AssessorService.Application.Api.Consts;
    using SFA.DAS.AssessorService.Application.Api.Validators;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.ViewModel.Models;

    public class ContactCreateViewModelValidatorTestBase
    {
        protected static ContactCreateViewModelValidator ContactCreateViewModelValidator;
        protected static Mock<IContactRepository> ContactRepositoryMock;      
        protected static CreateContactRequest ContactCreateViewModel;
        protected static Mock<IContactQueryRepository> ContactQueryRepository;

        public static void Setup()
        {
            var stringLocalizerMock = new Mock<IStringLocalizer<ContactCreateViewModelValidator>>();
            string key = ResourceMessageName.NoAssesmentProviderFound;
            var localizedString = new LocalizedString(key, "10000000");
            stringLocalizerMock.Setup(q => q[Moq.It.IsAny<string>(), Moq.It.IsAny<string>()]).Returns(localizedString);

            ContactRepositoryMock = new Mock<IContactRepository>();

            ContactQueryRepository = new Mock<IContactQueryRepository>();

            ContactCreateViewModelValidator = new ContactCreateViewModelValidator(stringLocalizerMock.Object, ContactQueryRepository.Object);               
        }
    }
}
