namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.UkPrnValidator
{
    using Microsoft.Extensions.Localization;
    using Moq;
    using SFA.DAS.AssessorService.Application.Api.Consts;
    using SFA.DAS.AssessorService.Application.Api.Validators;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class UkPrnValidatorTestBase
    {
        protected static UkPrnValidator UkPrnValidator;

        public static void Setup()
        {
            var stringLocalizerMock = new Mock<IStringLocalizer<UkPrnValidator>>();
            string key = ResourceMessageName.NoAssesmentProviderFound;
            var localizedString = new LocalizedString(key, "10000000");
            stringLocalizerMock.Setup(q => q[Moq.It.IsAny<string>(), Moq.It.IsAny<string>()]).Returns(localizedString);

            UkPrnValidator = new UkPrnValidator(stringLocalizerMock.Object);
        }
    }
}
