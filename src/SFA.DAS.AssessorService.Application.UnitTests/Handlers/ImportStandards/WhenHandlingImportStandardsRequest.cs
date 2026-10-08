using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.Standards;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.OuterApi;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.ImportStandards
{
    public class WhenHandlingImportStandardsRequest
    {
        const string ActiveStatus = "approved for delivery";
        const string DraftStatus = "in development";

        private readonly Fixture fixture = new Fixture();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IOuterApiService> _outerApiServiceMock = new Mock<IOuterApiService>();
        private readonly Mock<IStandardImportService> _standardServiceMock = new Mock<IStandardImportService>();
        private readonly Mock<ILogger<ImportStandardsHandler>> _loggerMock = new Mock<ILogger<ImportStandardsHandler>>();
        
        private List<StandardDetailResponse> _allStandardDetails;
        private ImportStandardsHandler _sut;

        [SetUp]
        public async Task Initialize()
        {
            _allStandardDetails = fixture.CreateMany<StandardDetailResponse>().ToList();
            _outerApiServiceMock.Setup(o => o.GetAllStandards()).ReturnsAsync(_allStandardDetails);
            
            _sut = new ImportStandardsHandler(_unitOfWorkMock.Object, _outerApiServiceMock.Object, _standardServiceMock.Object, _loggerMock.Object);

            await _sut.Handle(new ImportStandardsRequest(), new CancellationToken() );
        }

        [TearDown]
        public void ClearAll()
        {
            _allStandardDetails.Clear();
        }

        [Test]
        public void Then_Gets_All_Standards_From_Outer_Api()
        {
            _outerApiServiceMock.Verify(o => o.GetAllStandards());
        }

        [Test]
        public void Then_Deletes_Existing_Standards()
        {
            _standardServiceMock.Verify(s => s.PrepareImport(), Times.Once);
        }

        [Test]
        public void Then_Load_Standards()
        {
            _standardServiceMock.Verify(s => s.StageStandards(It.Is<IEnumerable<StandardDetailResponse>>(list => list.SequenceEqual(_allStandardDetails))));
        }
    }
}
