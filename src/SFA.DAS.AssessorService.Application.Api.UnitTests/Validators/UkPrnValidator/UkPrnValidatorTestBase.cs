using Microsoft.Extensions.Localization;
using Moq;
using SFA.DAS.AssessorService.Application.Api.Consts;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.UkPrnValidator
{
    public class UkPrnValidatorTestBase
    {
        protected static Api.Validators.UkPrnValidator UkPrnValidator;

        protected static void Setup()
        {
            var stringLocalizerMock = SetupStringLocaliserMock();
            UkPrnValidator = new Api.Validators.UkPrnValidator(stringLocalizerMock.Object);
        }

        private static Mock<IStringLocalizer<Api.Validators.UkPrnValidator>> SetupStringLocaliserMock()
        {
            var stringLocalizerMock = new Mock<IStringLocalizer<Api.Validators.UkPrnValidator>>();
            const string key = ResourceMessageName.NoAssesmentProviderFound;
            var localisedString = new LocalizedString(key, "10000000");
            stringLocalizerMock.Setup(q => q[It.IsAny<string>(), It.IsAny<string>()]).Returns(localisedString);
            return stringLocalizerMock;
        }
    }
}