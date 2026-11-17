using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.Standards;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.OuterApi;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.ImportStandards
{
    [TestFixture]
    public class WhenHandlingImportStandardsRequest
    {
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IOuterApiService> _outerApi;
        private Mock<IStandardImportService> _importService;
        private ImportStandardsHandler _sut;
        private List<StandardDetailResponse> _standards;

        [SetUp]
        public void SetUp()
        {
            _unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
            _outerApi = new Mock<IOuterApiService>(MockBehavior.Strict);
            _importService = new Mock<IStandardImportService>(MockBehavior.Strict);

            _standards = new List<StandardDetailResponse>
            {
                new StandardDetailResponse
                {
                    StandardUId = "ST0001_1.0"
                }
            };

            _outerApi.Setup(a => a.GetAllStandards())
                .ReturnsAsync(_standards);

            _unitOfWork.Setup(u => u.Begin());
            _unitOfWork.Setup(u => u.Commit());
            _unitOfWork.Setup(u => u.Rollback());

            _importService.Setup(s => s.PrepareImport())
                .Returns(Task.CompletedTask);

            _importService.Setup(s => s.StageStandards(_standards))
                .Returns(Task.CompletedTask);

            _importService.Setup(s => s.StageOptions(_standards))
                .Returns(Task.CompletedTask);

            _importService.Setup(s => s.MergeStandardsFromStaging())
                .Returns(Task.CompletedTask);

            _sut = new ImportStandardsHandler(
                _unitOfWork.Object,
                _outerApi.Object,
                _importService.Object,
                NullLogger<ImportStandardsHandler>.Instance);
        }

        [Test]
        public async Task Then_imports_the_standards_and_commits()
        {
            // Arrange
            var request = new ImportStandardsRequest();

            // Act
            await _sut.Handle(request, CancellationToken.None);

            // Assert
            _outerApi.Verify(a => a.GetAllStandards(), Times.Once);
            _importService.Verify(s => s.PrepareImport(), Times.Once);
            _importService.Verify(s => s.StageStandards(_standards), Times.Once);
            _importService.Verify(s => s.StageOptions(_standards), Times.Once);
            _importService.Verify(s => s.MergeStandardsFromStaging(), Times.Once);

            _unitOfWork.Verify(u => u.Begin(), Times.Once);
            _unitOfWork.Verify(u => u.Commit(), Times.Once);
            _unitOfWork.Verify(u => u.Rollback(), Times.Never);

            _outerApi.VerifyNoOtherCalls();
            _importService.VerifyNoOtherCalls();
            _unitOfWork.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Then_does_not_import_when_no_standards_are_returned()
        {
            // Arrange
            _standards.Clear();
            var request = new ImportStandardsRequest();

            // Act
            await _sut.Handle(request, CancellationToken.None);

            // Assert
            _outerApi.Verify(a => a.GetAllStandards(), Times.Once);
            _importService.VerifyNoOtherCalls();
            _unitOfWork.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Then_propagates_an_outer_api_failure_without_starting_a_transaction()
        {
            // Arrange
            var exception = new InvalidOperationException("Outer API failed");

            _outerApi.Setup(a => a.GetAllStandards())
                .ThrowsAsync(exception);

            var request = new ImportStandardsRequest();

            // Act
            Func<Task> act = () => _sut.Handle(request, CancellationToken.None);

            // Assert
            var result = await act.Should().ThrowAsync<InvalidOperationException>();
            result.Which.Should().BeSameAs(exception);

            _importService.VerifyNoOtherCalls();
            _unitOfWork.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Then_rolls_back_when_preparation_fails()
        {
            // Arrange
            var exception = new InvalidOperationException("Preparation failed");

            _importService.Setup(s => s.PrepareImport())
                .ThrowsAsync(exception);

            var request = new ImportStandardsRequest();

            // Act
            Func<Task> act = () => _sut.Handle(request, CancellationToken.None);

            // Assert
            var result = await act.Should().ThrowAsync<InvalidOperationException>();
            result.Which.Should().BeSameAs(exception);

            _unitOfWork.Verify(u => u.Begin(), Times.Once);
            _unitOfWork.Verify(u => u.Rollback(), Times.Once);
            _unitOfWork.Verify(u => u.Commit(), Times.Never);

            _importService.Verify(
                s => s.StageStandards(It.IsAny<IEnumerable<StandardDetailResponse>>()),
                Times.Never);

            _importService.Verify(
                s => s.StageOptions(It.IsAny<IEnumerable<StandardDetailResponse>>()),
                Times.Never);

            _importService.Verify(s => s.MergeStandardsFromStaging(), Times.Never);
        }

        [Test]
        public async Task Then_rolls_back_when_staging_standards_fails()
        {
            // Arrange
            var exception = new InvalidOperationException("Standards staging failed");

            _importService.Setup(s => s.StageStandards(_standards))
                .ThrowsAsync(exception);

            var request = new ImportStandardsRequest();

            // Act
            Func<Task> act = () => _sut.Handle(request, CancellationToken.None);

            // Assert
            var result = await act.Should().ThrowAsync<InvalidOperationException>();
            result.Which.Should().BeSameAs(exception);

            _unitOfWork.Verify(u => u.Begin(), Times.Once);
            _unitOfWork.Verify(u => u.Rollback(), Times.Once);
            _unitOfWork.Verify(u => u.Commit(), Times.Never);

            _importService.Verify(
                s => s.StageOptions(It.IsAny<IEnumerable<StandardDetailResponse>>()),
                Times.Never);

            _importService.Verify(s => s.MergeStandardsFromStaging(), Times.Never);
        }

        [Test]
        public async Task Then_rolls_back_when_staging_options_fails()
        {
            // Arrange
            var exception = new InvalidOperationException("Options staging failed");

            _importService.Setup(s => s.StageOptions(_standards))
                .ThrowsAsync(exception);

            var request = new ImportStandardsRequest();

            // Act
            Func<Task> act = () => _sut.Handle(request, CancellationToken.None);

            // Assert
            var result = await act.Should().ThrowAsync<InvalidOperationException>();
            result.Which.Should().BeSameAs(exception);

            _unitOfWork.Verify(u => u.Begin(), Times.Once);
            _unitOfWork.Verify(u => u.Rollback(), Times.Once);
            _unitOfWork.Verify(u => u.Commit(), Times.Never);

            _importService.Verify(s => s.MergeStandardsFromStaging(), Times.Never);
        }

        [Test]
        public async Task Then_rolls_back_when_merging_fails()
        {
            // Arrange
            var exception = new InvalidOperationException("Merge failed");

            _importService.Setup(s => s.MergeStandardsFromStaging())
                .ThrowsAsync(exception);

            var request = new ImportStandardsRequest();

            // Act
            Func<Task> act = () => _sut.Handle(request, CancellationToken.None);

            // Assert
            var result = await act.Should().ThrowAsync<InvalidOperationException>();
            result.Which.Should().BeSameAs(exception);

            _unitOfWork.Verify(u => u.Begin(), Times.Once);
            _unitOfWork.Verify(u => u.Rollback(), Times.Once);
            _unitOfWork.Verify(u => u.Commit(), Times.Never);
        }

        [Test]
        public async Task Then_rolls_back_when_committing_fails()
        {
            // Arrange
            var exception = new InvalidOperationException("Commit failed");

            _unitOfWork.Setup(u => u.Commit())
                .Throws(exception);

            var request = new ImportStandardsRequest();

            // Act
            Func<Task> act = () => _sut.Handle(request, CancellationToken.None);

            // Assert
            var result = await act.Should().ThrowAsync<InvalidOperationException>();
            result.Which.Should().BeSameAs(exception);

            _unitOfWork.Verify(u => u.Begin(), Times.Once);
            _unitOfWork.Verify(u => u.Commit(), Times.Once);
            _unitOfWork.Verify(u => u.Rollback(), Times.Once);
        }
    }
}