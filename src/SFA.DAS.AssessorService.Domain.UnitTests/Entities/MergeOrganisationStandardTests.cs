using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.AssessorService.Domain.UnitTests.Entities
{
    public class MergeOrganisationStandardTests
    {
        [Test, RecursiveMoqAutoData]
        public void When_Constructed_From_OrganisationStandard_Then_Values_Should_Be_Copied(OrganisationStandard sourceObject)
        {
            var objectUnderTest = new MergeOrganisationStandard(sourceObject, "Before");

            objectUnderTest.Replicates.Should().Be("Before");
            objectUnderTest.EndPointAssessorOrganisationId.Should().Be(sourceObject.EndPointAssessorOrganisationId);
            objectUnderTest.StandardCode.Should().Be(sourceObject.StandardCode);
            objectUnderTest.StandardReference.Should().Be(sourceObject.StandardReference);
            objectUnderTest.EffectiveFrom.Should().Be(sourceObject.EffectiveFrom);
            objectUnderTest.EffectiveTo.Should().Be(sourceObject.EffectiveTo);
            objectUnderTest.DateStandardApprovedOnRegister.Should().Be(sourceObject.DateStandardApprovedOnRegister);
            objectUnderTest.Comments.Should().Be(sourceObject.Comments);
            objectUnderTest.Status.Should().Be(sourceObject.Status);
            objectUnderTest.ContactId.Should().Be(sourceObject.ContactId);
            objectUnderTest.OrganisationStandardData.Should().Be(sourceObject.OrganisationStandardData);
            objectUnderTest.OrganisationStandardId.Should().Be(sourceObject.Id);
        }
    }
}