using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.BatchLogs;
using SFA.DAS.AssessorService.Data;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.DeleteCertificateHandlerTests
{
    [TestFixture]
    /*class When_called_to_add_certificates_to_batch
    {
        private Mock<IDbConnection> _mockConnection;
        private Mock<ICertificateRepository> _mockCertificateRepository;
        private Mock<IBatchLogRepository> _mockBatchLogRepository;
        private Mock<ILogger<UpdateBatchLogReadyToPrintAddCertificatesHandler>> _mockLogger;
        private AssessorUnitOfWork _unitOfWork;
        private UpdateBatchLogReadyToPrintAddCertificatesHandler _sut;  // Replace with your actual handler class name
        private AssessorDbContext _dbContext;

        private Guid[] _certificateIds;
        private int _batchNumber = 10;
        
        [SetUp]
        public void SetUp()
        {
            _mockConnection = new Mock<IDbConnection>();
            _mockConnection.Setup(c => c.State).Returns(ConnectionState.Open);

            var options = new DbContextOptionsBuilder<AssessorDbContext>()
                .UseInMemoryDatabase("TestDb") // Use an in-memory database for testing
                .Options;

            _dbContext = new AssessorDbContext(_mockConnection.Object, options);

            _unitOfWork = new AssessorUnitOfWork(_dbContext);

            _mockCertificateRepository = new Mock<ICertificateRepository>();
            _mockBatchLogRepository = new Mock<IBatchLogRepository>();
            _mockLogger = new Mock<ILogger<UpdateBatchLogReadyToPrintAddCertificatesHandler>>();

            _sut = new UpdateBatchLogReadyToPrintAddCertificatesHandler(
                _mockBatchLogRepository.Object,
                _mockCertificateRepository.Object,
                _unitOfWork,
                _mockLogger.Object
            );
        }

        [TestCase(10)]
        [TestCase(20)]
        [TestCase(50)]
        public async Task Then_next_certificates_ready_to_print_are_collected(int maxCertificatesToBeAdded)
        {
            // Act
            await _sut.Handle(new UpdateBatchLogReadyToPrintAddCertificatesRequest { MaxCertificatesToBeAdded = maxCertificatesToBeAdded }, CancellationToken.None);

            // Assert
            _mockCertificateRepository.Verify(v => v.GetCertificatesReadyToPrint(maxCertificatesToBeAdded, It.IsAny<string[]>(), It.IsAny<string[]>()), Times.Once);
        }

        [Test]
        public async Task Then_next_certificates_ready_to_print_valid_included_status()
        {
            // Act
            await _sut.Handle(new UpdateBatchLogReadyToPrintAddCertificatesRequest { MaxCertificatesToBeAdded = 1 }, CancellationToken.None);

            // Assert
            _mockCertificateRepository.Verify(v => v.GetCertificatesReadyToPrint(
                It.IsAny<int>(), 
                It.IsAny<string[]>(),
                It.Is<string[]>(s => s.Contains("Submitted") && s.Contains("Reprint") && s.Length == 2)), 
                Times.Once);
        }

        [Test]
        public async Task Then_next_certificates_ready_to_print_valid_excluded_overall_grades()
        {
            // Act
            await _sut.Handle(new UpdateBatchLogReadyToPrintAddCertificatesRequest { BatchNumber = _batchNumber, MaxCertificatesToBeAdded = 1 }, CancellationToken.None);

            // Assert
            _mockCertificateRepository.Verify(v => v.GetCertificatesReadyToPrint(
                It.IsAny<int>(),
                It.Is<string[]>(s => s.Contains("Fail") && s.Length == 1),
                It.IsAny<string[]>()),
                Times.Once);
        }

        [Test]
        public async Task Then_certifcate_batch_logs_are_updated()
        {
            // Act
            await _sut.Handle(new UpdateBatchLogReadyToPrintAddCertificatesRequest { BatchNumber = _batchNumber, MaxCertificatesToBeAdded = 1 }, CancellationToken.None);

            // Assert
            _mockBatchLogRepository.Verify(v => v.UpsertCertificatesReadyToPrintInBatch(_batchNumber, _certificateIds));
        }

        [Test]
        public async Task Then_certifcates_are_updated()
        {
            // Act
            await _sut.Handle(new UpdateBatchLogReadyToPrintAddCertificatesRequest { BatchNumber = _batchNumber, MaxCertificatesToBeAdded = 1 }, CancellationToken.None);

            // Assert
            _mockCertificateRepository.Verify(v => v.UpdateCertificatesReadyToPrintInBatch(_certificateIds, _batchNumber));
        }

        [Test]
        public async Task Then_unit_of_work_is_used()
        {
            // Arrange
            var mockAssessorUnitOfWork = new Mock<IAssessorUnitOfWork>();
            _sut = new UpdateBatchLogReadyToPrintAddCertificatesHandler(
                _mockBatchLogRepository.Object,
                _mockCertificateRepository.Object,
                mockAssessorUnitOfWork.Object,
                _mockLogger.Object);

            // Act
            await _sut.Handle(new UpdateBatchLogReadyToPrintAddCertificatesRequest { BatchNumber = _batchNumber, MaxCertificatesToBeAdded = 1 }, CancellationToken.None);

            // Assert
            mockAssessorUnitOfWork.Verify(v => v.ExecuteInTransactionAsync(It.IsAny<Func<Task<int>>>(), CancellationToken.None));
        }
    }*/
}
