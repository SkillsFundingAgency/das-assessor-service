using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Web.ViewModels.Standard;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.AssessorService.Web.UnitTests.ViewModelTests.StandardDetailsViewModelTests
{
    public class WhenCallingCannotOptOut
    {
        [Test, RecursiveMoqAutoData]
        public void And_TheGivenVersionIsTheOnlyApprovedOne_ReturnsTrue(StandardVersion version)
        {
            var sut = new StandardDetailsViewModel() { ApprovedVersions = new List<StandardVersion>() { version } };

            Assert.That(sut.CannotOptOut(version), Is.True);
        }

        [Test, RecursiveMoqAutoData]
        public void And_MultipleApprovedVersionsExist_ReturnsFalse(StandardVersion versionOne, StandardVersion versionTwo)
        {
            var sut = new StandardDetailsViewModel() 
            { 
                ApprovedVersions = new List<StandardVersion>() { versionOne, versionTwo } 
            };

            using (new AssertionScope())
            {
                sut.CannotOptOut(versionOne).Should().BeFalse();
                sut.CannotOptOut(versionTwo).Should().BeFalse();
            }
        }

        [Test, RecursiveMoqAutoData]
        public void And_NoApprovedVersionsExist_ReturnsFalse(StandardVersion version)
        {
            var sut = new StandardDetailsViewModel() { ApprovedVersions = new List<StandardVersion>() };

            Assert.That(sut.CannotOptOut(version), Is.False);
        }
    }
}
