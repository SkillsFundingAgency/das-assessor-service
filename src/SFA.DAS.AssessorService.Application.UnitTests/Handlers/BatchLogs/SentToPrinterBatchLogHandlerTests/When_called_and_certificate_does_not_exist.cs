using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.BatchLogs.SentToPrinterBatchLogHandlerTests
{
    public class When_called_and_certificate_does_not_exist : SentToPrinterBatchLogHandlerTestsBase
    {
        private static List<string> _certificateReferences = new List<string>
        {
            _certificateReference1,
            _certificateReference2,
            _certificateReference3
        };

        [SetUp]
        public async Task Arrange()
        {
            base.BaseArrange();

            _response = await _sut.Handle(
                new SentToPrinterBatchLogRequest { BatchNumber = _batchNumber, CertificateReferences = _certificateReferences },
                new CancellationToken());
        }

        [Test]
        public void Then_validation_response_is_valid_false()
        {
            _response.IsValid.Should().Be(false);
            _response.Errors.Count.Should().Be(1);
            _response.Errors[0].ErrorMessage.Contains(_certificateReference3);
        }

        [Test]
        public void Then_update_sent_to_print_repository_action()
        {
            _certificateRepository.Verify(r => r.UpdateSentToPrinter(
                It.Is<Certificate>(c => c.CertificateReference == _certificateReference1),
                It.IsAny<int>(), It.IsAny<DateTime>()));

            _certificateRepository.Verify(r => r.UpdateSentToPrinter(
                It.Is<Certificate>(c => c.CertificateReference == _certificateReference2),
                It.IsAny<int>(), It.IsAny<DateTime>()));
        }
    }
}
