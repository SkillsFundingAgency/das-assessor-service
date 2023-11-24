using System.Collections.Generic;
using System.Linq;
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

            Assert.Multiple(() =>
            {
                Assert.That(result.Count, Is.EqualTo(3));
                Assert.That(result.Any(r => r.StandardUId == "ST0001_1.0"), Is.True);
                Assert.That(result.Any(r => r.StandardUId == "ST0001_1.1"), Is.True);
                Assert.That(result.Any(r => r.StandardUId == "ST0001_1.2"), Is.True);
            });
        }

        [Test]
        public void DistinctResults_ReturnsNull_IfResultIsNull()
        {
            var sut = new ApplyStandardConfirmViewModel() { Results = null };

            Assert.That(sut.DistinctResults, Is.Null);
        }

        [Test]
        public void DistinctResults_ReturnsEmpty_IfResultIsNull()
        {
            var sut = new ApplyStandardConfirmViewModel() { Results = new List<StandardVersion>() };

            Assert.That(sut.DistinctResults, Is.Empty);
        }
    }
}
