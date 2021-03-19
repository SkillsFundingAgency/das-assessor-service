using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
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
    public class WhenHanldingImportStandardsRequest
    {
        const string ActiveStatus = "approved for delivery";
        const string DraftStatus = "in development";
        Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        Mock<IOuterApiService> _outerApiServiceMock = new Mock<IOuterApiService>();
        Mock<IStandardImportService> _standardServiceMock = new Mock<IStandardImportService>();
        Mock<ILogger<ImportStandardsHandler>> _loggerMock = new Mock<ILogger<ImportStandardsHandler>>();
        List<GetStandardsListItem> _allStandards = new List<GetStandardsListItem>();
        List<GetStandardByIdResponse> _allStandardDetails;
        ImportStandardsHandler _sut;

        [SetUp, AutoData]
        public async Task Initialize()
        {
            var fixture = new Fixture();
            var activeStandards = fixture.Create<List<GetStandardsListItem>>();
            var draftStandards = fixture.Create<List<GetStandardsListItem>>();
            var otherStandards = fixture.Create<List<GetStandardsListItem>>();

            activeStandards.ForEach(s => s.Status = ActiveStatus);
            draftStandards.ForEach(s => s.Status = DraftStatus);

            _allStandards.AddRange(activeStandards);
            _allStandards.AddRange(draftStandards);
            _allStandards.AddRange(otherStandards);

            _allStandardDetails = _allStandards.Select(ConvertToGetStandardByIdResponse).ToList();

            _outerApiServiceMock.Setup(o => o.GetAllStandards()).ReturnsAsync(_allStandards);
            _outerApiServiceMock.Setup(o => o.GetActiveStandards()).ReturnsAsync(activeStandards);
            _outerApiServiceMock.Setup(o => o.GetDraftStandards()).ReturnsAsync(draftStandards);
            _outerApiServiceMock.Setup(o => o.GetAllStandardDetails(It.IsAny<IEnumerable<string>>())).ReturnsAsync(_allStandardDetails);

            _sut = new ImportStandardsHandler(_unitOfWorkMock.Object, _outerApiServiceMock.Object, _standardServiceMock.Object, _loggerMock.Object);

            await _sut.Handle(new ImportStandardsRequest(), new CancellationToken() );
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
            _outerApiServiceMock.Verify(o => o.GetActiveStandards());
        }

        [Test]
        public void Then_Gets_Details_For_All_Standards_From_Outer_Api()
        {
            _outerApiServiceMock.Verify(o => o.GetAllStandardDetails(It.Is<IEnumerable<string>>(list => list.SequenceEqual(_allStandards.Select(s => s.StandardUId)) )));
        }

        [Test]
        public void Then_Load_Standards()
        {
            _standardServiceMock.Verify(s => s.LoadStandards(It.Is<IEnumerable<GetStandardByIdResponse>>(list => list.SequenceEqual(_allStandardDetails))));
        }

        [Test]
        public void Then_Upserts_StandardCollations()
        {
            _standardServiceMock.Verify(s => s.UpsertStandardCollations(It.Is<IEnumerable<GetStandardByIdResponse>>(list => list.SequenceEqual(_allStandardDetails.Where(d => d.Status == ActiveStatus)))));
        }

        [Test]
        public void Then_Upserts_StandardNonApprovedCollations()
        {
            _standardServiceMock.Verify(s => s.UpsertStandardNonApprovedCollations(It.Is<IEnumerable<GetStandardByIdResponse>>(list => list.SequenceEqual(_allStandardDetails.Where(d => d.Status == DraftStatus)))));
        }

        private GetStandardByIdResponse ConvertToGetStandardByIdResponse(GetStandardsListItem source) => new GetStandardByIdResponse
        {
            StandardUId = source.StandardUId,
            Status = source.Status
        };
    }
}
