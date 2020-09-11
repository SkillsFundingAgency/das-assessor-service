using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Handlers.BatchLogs;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.BatchLogs.PrintedBatchLogHandlerTests
{
    public class When_called_and_processed_date_earlier_than_batch_date
    {
        private Mock<IBatchLogQueryRepository> _batchLogQueryRepository;
        private Mock<ICertificateRepository> _certificateRepository;
        private Mock<IMediator> _mediator;
        private Mock<ILogger<PrintedBatchLogHandler>> _logger;

        private static int _batchNumber = 222;
        private static DateTime _printedAt = DateTime.UtcNow;

        private static string _certificateReference1 = "00000001";
        private static string _certificateReference2 = "00000002";

        private BatchLog _batchLog = new BatchLog { Id = Guid.NewGuid(), BatchNumber = _batchNumber, BatchCreated = DateTime.UtcNow.AddDays(1)};
        private List<Certificate> _certificates = new List<Certificate>
        {
            new Certificate
            {
                Id = Guid.NewGuid(),
                CertificateReference = _certificateReference1
            },
            new Certificate
            {
                Id = Guid.NewGuid(),
                CertificateReference = _certificateReference2
            }
        };


        private ValidationResponse _response;
        private UpdateCertificatesPrintStatusRequest _request;

        [SetUp]
        public async Task Arrange()
        {
            MappingBootstrapper.Initialize();

            _batchLogQueryRepository = new Mock<IBatchLogQueryRepository>();
            _batchLogQueryRepository.Setup(r => r.GetForBatchNumber(It.IsAny<int>())).Returns(Task.FromResult(_batchLog));

            _certificateRepository = new Mock<ICertificateRepository>();
            _certificateRepository.Setup(r => r.GetCertificatesForBatchLog(It.IsAny<int>())).Returns(Task.FromResult(_certificates));

            _mediator = new Mock<IMediator>();
            _mediator.Setup(r => r.Send(It.IsAny<UpdateCertificatesPrintStatusRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new ValidationResponse()))
                .Callback((UpdateCertificatesPrintStatusRequest request, CancellationToken cancellationToken) => { _request = request; });

            _logger = new Mock<ILogger<PrintedBatchLogHandler>>();

            var sut = new PrintedBatchLogHandler(_batchLogQueryRepository.Object, _certificateRepository.Object, _mediator.Object, _logger.Object);

            _response = await sut.Handle(new PrintedBatchLogRequest { BatchNumber = _batchNumber, PrintedAt = _printedAt }, new CancellationToken());
        }

        [Test]
        public void Then_validation_response_is_valid_false()
        {
            _response.IsValid.Should().Be(false);
            _response.Errors.Count.Should().Be(1);
        }

        [Test]
        public void Then_certificate_update_print_status_message_not_sent()
        {
            _mediator.Verify(r => r.Send(It.IsAny<UpdateCertificatesPrintStatusRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
