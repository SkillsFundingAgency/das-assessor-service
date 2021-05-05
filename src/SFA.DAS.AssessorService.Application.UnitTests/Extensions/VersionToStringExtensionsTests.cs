using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Extensions;

namespace SFA.DAS.AssessorService.Application.UnitTests.Extensions
{
    public class VersionToStringExtensionsTests
    {
        [TestCase(.0, "0.0")]
        [TestCase(0.0, "0.0")]
        [TestCase(1, "1.0")]
        [TestCase(1.0, "1.0")]
        [TestCase(1.1, "1.1")]
        public void When_ConvertingVersionDecimalToString_Then_ReturnStringWithAtleaseOneDecimalPlace(decimal? versionDecimal, string expected)
        {
            var result = versionDecimal.VersionToString();

            result.Should().Be(expected);
        }
    }
}
