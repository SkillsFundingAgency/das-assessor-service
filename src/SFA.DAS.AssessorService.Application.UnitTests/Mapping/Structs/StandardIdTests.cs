using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Mapping.Structs;

namespace SFA.DAS.AssessorService.Application.UnitTests.Mapping.Structs
{
    public class StandardIdTests
    {
        public Fixture fixture { get; set; }

        [SetUp]
        public void Initialise()
        {
            fixture = new Fixture();
        }

        [Test, AutoData]
        public void WhenCreatingStandardId_With_LarsCode_SetsUpCorrectly(int larsCode)
        {
            var id = new StandardId(larsCode.ToString());

            id.IdType.Should().Be(StandardId.StandardIdType.LarsCode);
            id.LarsCode.Should().Be(larsCode);

            id.IFateReferenceNumber.Should().BeNull();
            id.StandardUId.Should().BeNull();
        }

        [Test, AutoData]
        public void WhenCreatingStandardId_With_IFateReferenceNumber_SetsUpCorrectly()
        {
            string ifateReferenceNumber = string.Concat(fixture.Create<string>().Substring(0, 5),"A");
            var id = new StandardId(ifateReferenceNumber);

            id.IdType.Should().Be(StandardId.StandardIdType.IFateReferenceNumber);
            id.IFateReferenceNumber.Should().Be(ifateReferenceNumber);

            id.LarsCode.Should().Be(-1);
            id.StandardUId.Should().BeNull();
        }

        [Test, AutoData]
        public void WhenCreatingStandardId_With_AStringThatIsnotIfateRef_DefaultsToStandardUId(string standardUId)
        {
            var id = new StandardId(standardUId);

            id.IdType.Should().Be(StandardId.StandardIdType.StandardUId);
            id.StandardUId.Should().Be(standardUId);

            id.LarsCode.Should().Be(-1);
            id.IFateReferenceNumber.Should().BeNull();
        }
    }
}