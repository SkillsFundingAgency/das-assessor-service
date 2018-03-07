using Microsoft.Extensions.Localization;
using Moq;
using SFA.DAS.AssessorService.Application.Api.UnitTests.Externsions;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Helpers
{
    public class MockStringLocaliserWithoutParamsBuilder
    {
        private string _key  = "DefaultKey";
        private string _keyValue = "DefaultValue";

        public MockStringLocaliserWithoutParamsBuilder WithKey(string key)
        {
            _key = key;
            return this;
        }

        public MockStringLocaliserWithoutParamsBuilder WithKeyValue(string keyValue)
        {
            _keyValue = keyValue;
            return this;
        }

        public Mock<IStringLocalizer<TResourceType>> Build<TResourceType>()
        {
            var localiser = new Mock<IStringLocalizer<TResourceType>>();
            var localizedString = new LocalizedString(_key, _keyValue);
            localiser.Setup(q => q[Moq.It.IsAny<string>()]).Returns(localizedString);

            return localiser;
        }
    }
}
