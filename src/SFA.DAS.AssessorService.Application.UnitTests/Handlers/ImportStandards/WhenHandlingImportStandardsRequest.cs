using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.Standards;
using SFA.DAS.AssessorService.Application.Infrastructure.OuterApi;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.ImportStandards
{
    public class WhenHandlingImportStandardsRequest
    {
        Fixture fixture = new Fixture();
        const string ActiveStatus = "approved for delivery";
        const string DraftStatus = "in development";
        Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        Mock<IOuterApiService> _outerApiServiceMock = new Mock<IOuterApiService>();
        Mock<IStandardImportService> _standardServiceMock = new Mock<IStandardImportService>();
        Mock<ILogger<ImportStandardsHandler>> _loggerMock = new Mock<ILogger<ImportStandardsHandler>>();
        List<GetStandardsListItem> _allStandards;
        List<StandardDetailResponse> _allStandardDetails;
        ImportStandardsHandler _sut;

        [SetUp]
        public async Task Initialize()
        {
            var activeStandards = fixture.Build<GetStandardsListItem>().With(t => t.Status, ActiveStatus).CreateMany();
            var draftStandards = fixture.Build<GetStandardsListItem>().With(t => t.Status, DraftStatus).CreateMany();
            var otherStandards = fixture.CreateMany<GetStandardsListItem>();

            _allStandards = new List<GetStandardsListItem>();
            _allStandards.AddRange(activeStandards);
            _allStandards.AddRange(draftStandards);
            _allStandards.AddRange(otherStandards);
            _allStandardDetails = _allStandards.Select(ConvertToStandardDetailResponse).ToList();

            _outerApiServiceMock.Setup(o => o.GetAllStandards()).ReturnsAsync(_allStandardDetails);
            _outerApiServiceMock.Setup(o => o.GetActiveStandards()).ReturnsAsync(activeStandards);
            _outerApiServiceMock.Setup(o => o.GetDraftStandards()).ReturnsAsync(draftStandards);

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
        public void Then_Gets_Active_Standards_From_Outer_Api()
        {
            _outerApiServiceMock.Verify(o => o.GetActiveStandards());
        }

        [Test]
        public void Then_Gets_Draft_Standards_From_Outer_Api()
        {
            _outerApiServiceMock.Verify(o => o.GetDraftStandards());
        }

        [Test]
        public void Then_Deletes_Existing_Standards()
        {
            _standardServiceMock.Verify(s => s.DeleteAllStandardsAndOptions(), Times.Once);
        }

        [Test]
        public void Then_Load_Standards()
        {
            _standardServiceMock.Verify(s => s.LoadStandards(It.Is<IEnumerable<StandardDetailResponse>>(list => list.SequenceEqual(_allStandardDetails))));
        }

        [Test]
        public void Then_Upserts_StandardCollations()
        {
            _standardServiceMock.Verify(s => s.UpsertStandardCollations(It.Is<IEnumerable<StandardDetailResponse>>(list => list.SequenceEqual(_allStandardDetails.Where(d => d.Status == ActiveStatus)))));
        }

        [Test]
        public void Then_Upserts_StandardNonApprovedCollations()
        {
            _standardServiceMock.Verify(s => s.UpsertStandardNonApprovedCollations(It.Is<IEnumerable<StandardDetailResponse>>(list => list.SequenceEqual(_allStandardDetails.Where(d => d.Status == DraftStatus)))));
        }

        private StandardDetailResponse ConvertToStandardDetailResponse(GetStandardsListItem source) => new StandardDetailResponse
        {
            StandardUId = source.StandardUId,
            Status = source.Status
        };
    }
}
