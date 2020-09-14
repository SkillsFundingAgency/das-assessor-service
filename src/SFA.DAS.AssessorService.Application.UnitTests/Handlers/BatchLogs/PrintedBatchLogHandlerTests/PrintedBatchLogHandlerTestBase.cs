using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Handlers.BatchLogs;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.BatchLogs.PrintedBatchLogHandlerTests
{
    public class PrintedBatchLogHandlerTestBase
    {
        protected Mock<IBatchLogQueryRepository> _batchLogQueryRepository;
        protected Mock<ICertificateRepository> _certificateRepository;
        protected Mock<IMediator> _mediator;
        protected Mock<ILogger<PrintedBatchLogHandler>> _logger;

        protected static int _batchNumber = 222;
        protected static DateTime _printedAt = DateTime.UtcNow;

        protected static string _certificateReference1 = "00000001";
        protected static string _certificateReference2 = "00000002";        
        
        protected List<Certificate> _certificates = new List<Certificate>
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

        protected UpdateCertificatesPrintStatusRequest _request;

        public void BaseArrange()
        {   
            MappingBootstrapper.Initialize();
            
            _certificateRepository = new Mock<ICertificateRepository>();
            _certificateRepository.Setup(r => r.GetCertificatesForBatchLog(It.IsAny<int>())).Returns(Task.FromResult(_certificates));

            _mediator = new Mock<IMediator>();
            _mediator.Setup(r => r.Send(It.IsAny<UpdateCertificatesPrintStatusRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new ValidationResponse()))
                .Callback((UpdateCertificatesPrintStatusRequest request, CancellationToken cancellationToken) => { _request = request; });

            _logger = new Mock<ILogger<PrintedBatchLogHandler>>();           
        }
    }
}
