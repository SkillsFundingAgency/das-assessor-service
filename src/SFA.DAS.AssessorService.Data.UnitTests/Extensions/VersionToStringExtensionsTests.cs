using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.Extensions;

namespace SFA.DAS.AssessorService.Data.UnitTests.Extensions
{
    public class VersionToStringExtensionsTests
    {
        [TestCase(0.0, "0.0")]
        [TestCase(1, "1.0")]
        [TestCase(1.1, "1.1")]
        public void When_ConvertingVersionDecimalToString_Then_ReturnStringWithAtleaseOneDecimalPlace(decimal? versionDecimal, string expected)
        {
            var result = versionDecimal.VersionToString();

            result.Should().Be(expected);
        }
    }
}
