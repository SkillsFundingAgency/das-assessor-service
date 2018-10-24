﻿using Microsoft.Extensions.Localization;
using Moq;

namespace SFA.DAS.AssessorService.Web.Staff.Tests.Helpers
{
    public class MockStringLocaliserBuilder
    {
        private string _key = "DefaultKey";
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

        public Mock<IStringLocalizer<TResourceType>> Build<TResourceType>()
        {
            var localiser = new Mock<IStringLocalizer<TResourceType>>();
            var localizedString = new LocalizedString(_key, _keyValue);
            localiser.Setup(q => q[Moq.It.IsAny<string>()]).Returns(localizedString);

            return localiser;
        }
    }
}
