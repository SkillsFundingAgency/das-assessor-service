using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.CoronationEmblemDataUpdaterTests
{
    public class CertificateDataVersionChangeUpdaterTests
    {
        [TestCase("1.1", "1.2", 1)]
        [TestCase("1.4", "1.3", 1)]
        [TestCase("1.1", "1.1", 0)]
        public async Task Retrieves_NewCoronationEmblemAndStandardName_OnlyIf_VersionHasChanged(string oldVersion, string newVersion, int expectedCallCount)
        {
            var standardRepositoryMock = new Mock<IStandardRepository>();
            var currentCertificateData = new CertificateData() { StandardReference = "doesn't matter", Version = oldVersion };
            var newCertificateData = new CertificateData() { StandardReference = "doesn't matter", Version = newVersion };

            await CertificateDataVersionChangeUpdater.UpdateCoronationEmblemAndStandardIfNeeded(currentCertificateData, newCertificateData, standardRepositoryMock.Object);

            using (new AssertionScope())
            {
                standardRepositoryMock.Verify(
                    s => s.GetCoronationEmblemForStandardReferenceAndVersion(newCertificateData.StandardReference, newCertificateData.Version),
                    Times.Exactly(expectedCallCount));

                standardRepositoryMock.Verify(
                    s => s.GetTitleForStandardReferenceAndVersion(newCertificateData.StandardReference, newCertificateData.Version),
                    Times.Exactly(expectedCallCount));
            }
        }

        [TestCase("1.1", "1.2", true, "new name")] 
        [TestCase("1.4", "1.3", true, "new name")] 
        [TestCase("1.1", "1.1", false, null)] // version hasn't changed, so expect the default values for the bool and string
        public async Task Sets_CoronationEmblemAndStandardName_ToNewlyRetrievedValues_OnlyIf_VersionHasChanged(
            string oldVersion, 
            string newVersion, 
            bool expectedCoronationEmblem, 
            string expectedStandardName)
        {
            var standardRepositoryMock = new Mock<IStandardRepository>();

            standardRepositoryMock.Setup(s => s.GetCoronationEmblemForStandardReferenceAndVersion(It.IsAny<string>(), It.IsAny<string>()))
                                  .ReturnsAsync(expectedCoronationEmblem);

            standardRepositoryMock.Setup(s => s.GetTitleForStandardReferenceAndVersion(It.IsAny<string>(), It.IsAny<string>()))
                                  .ReturnsAsync(expectedStandardName);

            var currentCertificateData = new CertificateData() { StandardReference = "doesn't matter", Version = oldVersion };
            var newCertificateData = new CertificateData() { StandardReference = "doesn't matter", Version = newVersion };

            var updatedData = await CertificateDataVersionChangeUpdater.UpdateCoronationEmblemAndStandardIfNeeded(currentCertificateData, newCertificateData, standardRepositoryMock.Object);

            using (new AssertionScope())
            {
                updatedData.CoronationEmblem.Should().Be(expectedCoronationEmblem);
                updatedData.StandardName.Should().Be(expectedStandardName);
            }
        }
    }
}
