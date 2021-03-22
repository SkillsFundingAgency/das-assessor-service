using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Services;
using SFA.DAS.AssessorService.Application.Infrastructure.OuterApi;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Services.StandardImportServiceTests
{
    public class WhenLoadingStandards
    {
        Mock<IStandardRepository> standardRepositoryMock;
        IEnumerable<GetStandardByIdResponse> standards;

        [SetUp]
        public async Task Initialize()
        {
            var fixture = new Fixture();
            standards = fixture.Build<GetStandardByIdResponse>().CreateMany();
            standardRepositoryMock = new Mock<IStandardRepository>();

            var sut = new StandardImportService(standardRepositoryMock.Object);

            await sut.LoadStandards(standards);
        }

        [Test]
        public void Then_Deletes_All_From_Standards_Table()
        {
            standardRepositoryMock.Verify(r => r.DeleteAll());
        }

        [Test]
        public void Then_Inserts_Data_Into_Standards_Table()
        {
            standardRepositoryMock.Verify(r => r.Insert(It.IsAny<Standard>()), Times.Exactly(standards.Count()));
        }
    }
}
