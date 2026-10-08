using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Services;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Services.StandardImportServiceTests
{
    [TestFixture]
    public class WhenPreparingAndMergingStandards
    {
        private Mock<IStandardRepository> _repository;
        private StandardImportService _sut;

        [SetUp]
        public void SetUp()
        {
            _repository = new Mock<IStandardRepository>(MockBehavior.Strict);
            _sut = new StandardImportService(_repository.Object);
        }

        [Test]
        public async Task Then_prepares_the_import_once()
        {
            // Arrange
            _repository.Setup(r => r.PrepareStandardsImport())
                .Returns(Task.CompletedTask);

            // Act
            await _sut.PrepareImport();

            // Assert
            _repository.Verify(r => r.PrepareStandardsImport(), Times.Once);
            _repository.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Then_merges_staging_once()
        {
            // Arrange
            _repository.Setup(r => r.MergeStandardsFromStaging())
                .Returns(Task.CompletedTask);

            // Act
            await _sut.MergeStandardsFromStaging();

            // Assert
            _repository.Verify(r => r.MergeStandardsFromStaging(), Times.Once);
            _repository.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Then_propagates_the_original_preparation_exception()
        {
            // Arrange
            var exception = new InvalidOperationException("Preparation failed");
            _repository.Setup(r => r.PrepareStandardsImport())
                .ThrowsAsync(exception);

            // Act
            Func<Task> act = () => _sut.PrepareImport();

            // Assert
            var result = await act.Should().ThrowAsync<InvalidOperationException>();
            result.Which.Should().BeSameAs(exception);
            _repository.Verify(r => r.PrepareStandardsImport(), Times.Once);
            _repository.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Then_propagates_the_original_merge_exception()
        {
            // Arrange
            var exception = new InvalidOperationException("Merge failed");
            _repository.Setup(r => r.MergeStandardsFromStaging())
                .ThrowsAsync(exception);

            // Act
            Func<Task> act = () => _sut.MergeStandardsFromStaging();

            // Assert
            var result = await act.Should().ThrowAsync<InvalidOperationException>();
            result.Which.Should().BeSameAs(exception);
            _repository.Verify(r => r.MergeStandardsFromStaging(), Times.Once);
            _repository.VerifyNoOtherCalls();
        }
    }
}