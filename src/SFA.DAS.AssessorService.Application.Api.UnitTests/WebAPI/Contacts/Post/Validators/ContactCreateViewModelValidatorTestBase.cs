using Microsoft.Extensions.Localization;
using Moq;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Contacts.Post.Validators
{
    public class ContactCreateViewModelValidatorTestBase
    {
        protected static ContactCreateViewModelValidator ContactCreateViewModelValidator;
        protected static Mock<IContactRepository> ContactRepositoryMock;      
        protected static CreateContactRequest ContactCreateViewModel;
        protected static Mock<IContactQueryRepository> ContactQueryRepository;

        public static void Setup()
        {
            var stringLocalizerMock = new Mock<IStringLocalizer<ContactCreateViewModelValidator>>();
            var key = ResourceMessageName.NoAssesmentProviderFound;
            var localizedString = new LocalizedString(key, "10000000");
            stringLocalizerMock.Setup(q => q[It.IsAny<string>(), It.IsAny<string>()]).Returns(localizedString);

            ContactRepositoryMock = new Mock<IContactRepository>();

            ContactQueryRepository = new Mock<IContactQueryRepository>();

            ContactCreateViewModelValidator = new ContactCreateViewModelValidator(stringLocalizerMock.Object, ContactQueryRepository.Object);               
        }
    }
}
