using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Services;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.OuterApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Services.StandardImportServiceTests
{
    public class WhenLoadingOptions
    {
        Mock<IStandardRepository> standardRepositoryMock;
        IEnumerable<StandardDetailResponse> standards;

        [SetUp]
        public async Task Initialize()
        {
            var fixture = new Fixture();
            standards = fixture.Build<StandardDetailResponse>().CreateMany();
            standardRepositoryMock = new Mock<IStandardRepository>();

            var sut = new StandardImportService(standardRepositoryMock.Object);

            await sut.LoadOptions(standards);
        }

        [Test]
        public void Then_Inserts_Data_Into_Standards_Table()
        {
            standardRepositoryMock.Verify(r => r.InsertOptions(It.IsAny<IEnumerable<StandardOption>>()), Times.Once);
        }
    }
}
