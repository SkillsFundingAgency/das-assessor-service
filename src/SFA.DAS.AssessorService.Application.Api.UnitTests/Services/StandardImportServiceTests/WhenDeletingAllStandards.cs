using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Services;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Services.StandardImportServiceTests
{
    public class WhenDeletingAllStandardsAndOptions
    {
        Mock<IStandardRepository> standardRepositoryMock;

        [SetUp]
        public async Task Initialize()
        {
            standardRepositoryMock = new Mock<IStandardRepository>();

            var sut = new StandardImportService(standardRepositoryMock.Object);

            await sut.DeleteAllStandardsAndOptions();
        }

        [Test]
        public void Then_Deletes_Data_From_Standards_Table_And_Options_Table()
        {
            standardRepositoryMock.Verify(r => r.DeleteAllStandards(), Times.Once);
            standardRepositoryMock.Verify(r => r.DeleteAllOptions(), Times.Once);
        }
    }
}
