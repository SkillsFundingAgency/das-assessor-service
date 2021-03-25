using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Services;
using SFA.DAS.AssessorService.Application.Infrastructure.OuterApi;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Services.StandardImportServiceTests
{
    public class WhenUpsertingStandardNonApprovedCollations
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

            await sut.UpsertStandardNonApprovedCollations(standards);
        }

        [Test]
        public void Then_Inserts_Data_Into_StandardNonApprovedCollation_Table()
        {
            standardRepositoryMock.Verify(r => r.UpsertNonApprovedStandards(It.IsAny<List<StandardNonApprovedCollation>>()));
        }
    }
}
