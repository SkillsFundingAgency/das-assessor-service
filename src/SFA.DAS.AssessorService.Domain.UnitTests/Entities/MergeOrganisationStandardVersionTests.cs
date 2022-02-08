using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.AssessorService.Domain.UnitTests.Entities
{
    public class MergeOrganisationStandardVersionTests
    {
        [Test, RecursiveMoqAutoData]
        public void When_Constructed_From_OrganisationStandardVersion_Then_Values_Should_Be_Copied(OrganisationStandardVersion sourceObject)
        {
            var objectUnderTest = new MergeOrganisationStandardVersion(sourceObject, "Before");

            objectUnderTest.Replicates.Should().Be("Before");
            objectUnderTest.StandardUid.Should().Be(sourceObject.StandardUId);
            objectUnderTest.Version.Should().Be(sourceObject.Version);
            objectUnderTest.OrganisationStandardId.Should().Be(sourceObject.OrganisationStandardId);
            objectUnderTest.EffectiveFrom.Should().Be(sourceObject.EffectiveFrom);
            objectUnderTest.EffectiveTo.Should().Be(sourceObject.EffectiveTo);
            objectUnderTest.Comments.Should().Be(sourceObject.Comments);
            objectUnderTest.Status.Should().Be(sourceObject.Status);
            objectUnderTest.DateVersionApproved.Should().Be(sourceObject.DateVersionApproved);
        }
    }
}