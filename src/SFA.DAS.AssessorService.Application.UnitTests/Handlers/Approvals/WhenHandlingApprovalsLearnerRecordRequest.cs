using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Learner;
using SFA.DAS.AssessorService.Application.Handlers.Approvals;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Approvals
{
    [TestFixture]
    public class WhenHandlingApprovalsLearnerRecordRequest
    {
        private Mock<ILogger<GetApprovalsLearnerRecordHandler>> _loggerMock;
        private Mock<ILearnerRepository> _learnerRepositoryMock;
        private GetApprovalsLearnerRecordHandler _sut;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<GetApprovalsLearnerRecordHandler>>();
            _learnerRepositoryMock = new Mock<ILearnerRepository>();

            _sut = new GetApprovalsLearnerRecordHandler(_learnerRepositoryMock.Object, _loggerMock.Object);
        }

        [Test, AutoData]
        public async Task ThenCallGetOnLearnerRepository(GetApprovalsLearnerRecordRequest request)
        {
            // Arrange.

            // Act.
            await _sut.Handle(request, new CancellationToken());

            // Assert.
            _learnerRepositoryMock.Verify(m => m.Get(request.Uln, request.StdCode));
        }

        [Test, AutoData]
        public async Task Then_Returns_Null_If_No_Learner_Found(GetApprovalsLearnerRecordRequest request)
        {
            // Arrange.

            // Act.
            var result = await _sut.Handle(request, new CancellationToken());

            // Assert.
            result.Should().BeNull();
        }

        [Test, AutoData]
        public async Task Then_Return_Learner_If_Found(GetApprovalsLearnerRecordRequest request, Domain.Entities.Learner learner)
        {
            // Arrange.
            _learnerRepositoryMock.Setup(s => s.Get(request.Uln, request.StdCode)).ReturnsAsync(learner);
            // Act.

            var result = await _sut.Handle(request, new CancellationToken());

            // Assert.
            result.Should().BeEquivalentTo(new
            {
                learner.Uln,
                learner.FamilyName,
                learner.GivenNames,
                StandardCode = learner.StdCode,
                learner.Version,
                learner.VersionConfirmed,
                learner.CourseOption
            });
        }
    }
}
