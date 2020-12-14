using System.Threading;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Application.Handlers.BatchLogs;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using System.Linq;
using System;
using FizzWare.NBuilder;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.DeleteCertificateHandlerTests
{
    [TestFixture]
    class When_called_to_add_certificates_to_batch
    {
        private Mock<ICertificateBatchLogRepository> _mockCertificateBatchLogRepository;
        private Mock<ICertificateRepository> _mockCertificateRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ILogger<UpdateBatchLogReadyToPrintAddCertificatesHandler>> _mockLogger;
        
        private UpdateBatchLogReadyToPrintAddCertificatesHandler _sut;

        private Guid[] _certificateIds;
        private int _batchNumber = 10;

        [SetUp]
        public void Arrange()
        {
            _mockCertificateBatchLogRepository = new Mock<ICertificateBatchLogRepository>();
            
            _mockCertificateRepository = new Mock<ICertificateRepository>();
            _mockCertificateRepository.Setup(c => c.Delete(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<string>(), CertificateActions.Delete, true, It.IsAny<string>(), It.IsAny<string>()))
               .Returns(() => Task.Run(() => { })).Verifiable();

            _certificateIds = Builder<Guid>.CreateListOfSize(10).All().Build().ToArray();
            _mockCertificateRepository.Setup(s => s.GetCertificatesReadyToPrint(It.IsAny<int>(), It.IsAny<string[]>(), It.IsAny<string[]>()))
                .ReturnsAsync(_certificateIds);

            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<UpdateBatchLogReadyToPrintAddCertificatesHandler>>();

            _sut = new UpdateBatchLogReadyToPrintAddCertificatesHandler(
                _mockCertificateBatchLogRepository.Object,
                _mockCertificateRepository.Object,
                _mockUnitOfWork.Object,
                _mockLogger.Object);
        }

        [TestCase(10)]
        [TestCase(20)]
        [TestCase(50)]
        public async Task Then_next_certificates_ready_to_print_are_collected(int maxCertificatesToBeAdded)
        {
            // Act
            await _sut.Handle(new UpdateBatchLogReadyToPrintAddCertificatesRequest { MaxCertificatesToBeAdded = maxCertificatesToBeAdded }, new CancellationToken());

            // Assert
            _mockCertificateRepository.Verify(v => v.GetCertificatesReadyToPrint(maxCertificatesToBeAdded, It.IsAny<string[]>(), It.IsAny<string[]>()), Times.Once);
        }

        [Test]
        public async Task Then_next_certificates_ready_to_print_valid_included_status()
        {
            // Act
            await _sut.Handle(new UpdateBatchLogReadyToPrintAddCertificatesRequest { MaxCertificatesToBeAdded = 1 }, new CancellationToken());

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
            await _sut.Handle(new UpdateBatchLogReadyToPrintAddCertificatesRequest { BatchNumber = _batchNumber, MaxCertificatesToBeAdded = 1 }, new CancellationToken());

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
            await _sut.Handle(new UpdateBatchLogReadyToPrintAddCertificatesRequest { BatchNumber = _batchNumber, MaxCertificatesToBeAdded = 1 }, new CancellationToken());

            // Assert
            _mockCertificateBatchLogRepository.Verify(v => v.UpdateCertificatesReadyToPrintInBatch(_certificateIds, _batchNumber));
        }

        [Test]
        public async Task Then_certifcates_are_updated()
        {
            // Act
            await _sut.Handle(new UpdateBatchLogReadyToPrintAddCertificatesRequest { BatchNumber = _batchNumber, MaxCertificatesToBeAdded = 1 }, new CancellationToken());

            // Assert
            _mockCertificateRepository.Verify(v => v.UpdateCertificatesReadyToPrintInBatch(_certificateIds, _batchNumber));
        }

        [Test]
        public async Task Then_unit_of_work_is_used()
        {
            // Act
            await _sut.Handle(new UpdateBatchLogReadyToPrintAddCertificatesRequest { BatchNumber = _batchNumber, MaxCertificatesToBeAdded = 1 }, new CancellationToken());

            // Assert
            _mockUnitOfWork.Verify(v => v.Begin());
            _mockUnitOfWork.Verify(v => v.Commit());
        }

    }
}
