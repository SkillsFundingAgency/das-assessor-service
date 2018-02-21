namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.ContactContoller.Put.Validators
{
    using Microsoft.Extensions.Localization;
    using Moq;
    using SFA.DAS.AssessorService.Application.Api.Consts;
    using SFA.DAS.AssessorService.Application.Api.Validators;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.ViewModel.Models;

    public class ContactUpdateViewModelValidatorTestBase
    {
        protected static ContactUpdateViewModelValidator ContactUpdateViewModelValidator;
        protected static Mock<IContactRepository> ContactRepositoryMock;      
        protected static ContactCreateViewModel ContactCreateViewModel;

        public static void Setup()
        {
            var stringLocalizerMock = new Mock<IStringLocalizer<ContactUpdateViewModelValidator>>();
            string key = ResourceMessageName.NoAssesmentProviderFound;
            var localizedString = new LocalizedString(key, "10000000");
            stringLocalizerMock.Setup(q => q[Moq.It.IsAny<string>(), Moq.It.IsAny<string>()]).Returns(localizedString);

            ContactRepositoryMock = new Mock<IContactRepository>();

            ContactUpdateViewModelValidator = new ContactUpdateViewModelValidator(stringLocalizerMock.Object);               
        }
    }
}
