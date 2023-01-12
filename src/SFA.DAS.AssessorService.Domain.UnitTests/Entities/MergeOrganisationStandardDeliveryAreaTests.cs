using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.AssessorService.Domain.UnitTests.Entities
{
    public class MergeOrganisationStandardDeliveryAreaTests
    {
        [Test, RecursiveMoqAutoData]
        public void When_Constructed_From_OrganisationStandardDeliveryArea_Then_Values_Should_Be_Copied(OrganisationStandardDeliveryArea sourceObject)
        {
            var objectUnderTest = new MergeOrganisationStandardDeliveryArea(sourceObject, "Before");

            objectUnderTest.Replicates.Should().Be("Before");
            objectUnderTest.DeliveryAreaId.Should().Be(sourceObject.DeliveryAreaId);
            objectUnderTest.OrganisationStandardId.Should().Be(sourceObject.OrganisationStandardId);
            objectUnderTest.Comments.Should().Be(sourceObject.Comments);
            objectUnderTest.Status.Should().Be(sourceObject.Status);
            objectUnderTest.OrganisationStandardDeliveryAreaId.Should().Be(sourceObject.Id);
        }
    }
}