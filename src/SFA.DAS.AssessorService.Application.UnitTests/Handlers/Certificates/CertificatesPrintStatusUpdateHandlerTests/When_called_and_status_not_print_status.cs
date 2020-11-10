using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.UpdateCertificatesPrintStatusHandlerTests
{
    public class When_called_and_status_not_print_status : UpdateCertificatesPrintStatusHandlerTestsBase
    {
        private ValidationResponse _response;
        private static List<string> _invalidPrintStatuses = new List<string>
        {
            CertificateStatus.Approved,
            CertificateStatus.Cancelled,
            CertificateStatus.Deleted,
            CertificateStatus.Draft,
            CertificateStatus.NoCertificate,
            CertificateStatus.Rejected,
            CertificateStatus.Reprint,
            CertificateStatus.SentForApproval,
            CertificateStatus.ToBeApproved
        };

        [SetUp]
        public async Task Arrange()
        {
            base.BaseArrange();

            var certificatePrintStatusUpdates = new List<CertificatePrintStatusUpdate>();
            _invalidPrintStatuses.ForEach(r =>
            {
                certificatePrintStatusUpdates.Add(
                    new CertificatePrintStatusUpdate
                    {
                        BatchNumber = _batch111,
                        CertificateReference = _certificateReference1,
                        Status = r,
                        StatusAt = DateTime.UtcNow
                    }
                );
            });

            _response = await _sut.Handle(
                new CertificatesPrintStatusUpdateRequest
                {
                    CertificatePrintStatusUpdates = certificatePrintStatusUpdates
                }, new CancellationToken());
        }

        [Test]
        public void Then_validation_response_is_valid_false()
        {
            _response.IsValid.Should().Be(false);
            _response.Errors.Count.Should().Be(_invalidPrintStatuses.Count);

            for(int error = 0; error < _invalidPrintStatuses.Count; error++)
            {
                _response.Errors[error].Field.Should().Be("CertificatePrintStatuses");
                _response.Errors.Should().Contain(p => p.ErrorMessage.Contains(_invalidPrintStatuses[error]));
            }
        }

        [Test]
        public void Then_repository_update_print_status_is_not_called()
        {
            _certificateRepository.Verify(r => r.UpdatePrintStatus(
                It.IsAny<Certificate>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<bool>()), 
                Times.Never());
        }
    }
}
