using Microsoft.Extensions.Localization;
using Moq;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Helpers
{
    public class MockStringLocaliserBuilder
    {
        private string _key  = "DefaultKey";
        private string _keyValue = "DefaultValue";

        public MockStringLocaliserBuilder WithKey(string key)
        {
            _key = key;
            return this;
        }

        public MockStringLocaliserBuilder WithKeyValue(string keyValue)
        {
            _keyValue = keyValue;
            return this;
        }

        public Mock<IStringLocalizer<T>> Build<T>()
        {
            var localiser = new Mock<IStringLocalizer<T>>();
            var localizedString = new LocalizedString(_key, _keyValue);
            localiser.Setup(q => q[Moq.It.IsAny<string>(), Moq.It.IsAny<string>()]).Returns(localizedString);

            return localiser;
        }
    }
}
