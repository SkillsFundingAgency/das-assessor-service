using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Web.ViewModels.Standard;

namespace SFA.DAS.AssessorService.Web.UnitTests.ViewModelTests
{
    public class ApplyStandardConfirmViewModelTests
    {
        [Test]
        public void DistinctResults_ReturnsDistinctStandardVersions()
        {
            var standardVersions = new List<StandardVersion>()
            {
                new StandardVersion(){ StandardUId = "ST0001_1.0" },
                new StandardVersion(){ StandardUId = "ST0001_1.1" },
                new StandardVersion(){ StandardUId = "ST0001_1.1" },
                new StandardVersion(){ StandardUId = "ST0001_1.2" },
                new StandardVersion(){ StandardUId = "ST0001_1.2" },
            };

            var sut = new ApplyStandardConfirmViewModel() { Results = standardVersions };

            var result = sut.DistinctResults;

            using (new AssertionScope())
            {
                result.Count.Should().Be(3);
                result.Should().Contain(r => r.StandardUId == "ST0001_1.0");
                result.Should().Contain(r => r.StandardUId == "ST0001_1.1");
                result.Should().Contain(r => r.StandardUId == "ST0001_1.2");
            };
        }

        [Test]
        public void DistinctResults_ReturnsNull_IfResultIsNull()
        {
            var sut = new ApplyStandardConfirmViewModel() { Results = null };

            sut.DistinctResults.Should().BeNull();
        }

        [Test]
        public void DistinctResults_ReturnsEmpty_IfResultIsNull()
        {
            var sut = new ApplyStandardConfirmViewModel() { Results = new List<StandardVersion>() };

            Assert.That(sut.DistinctResults, Is.Empty);
        }
    }
}
