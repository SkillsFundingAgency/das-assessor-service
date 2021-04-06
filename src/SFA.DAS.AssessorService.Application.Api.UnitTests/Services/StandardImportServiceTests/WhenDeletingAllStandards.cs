using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Services;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Services.StandardImportServiceTests
{
    public class WhenDeletingAllStandards
    {
        Mock<IStandardRepository> standardRepositoryMock;

        [SetUp]
        public async Task Initialize()
        {
            standardRepositoryMock = new Mock<IStandardRepository>();

            var sut = new StandardImportService(standardRepositoryMock.Object);

            await sut.DeleteAllStandards();
        }

        [Test]
        public void Then_Inserts_Data_Into_Standards_Table()
        {
            standardRepositoryMock.Verify(r => r.DeleteAll(), Times.Once);
        }
    }
}
