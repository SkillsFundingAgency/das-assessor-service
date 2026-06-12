using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Domain.Consts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.UpdateCertificatesPrintStatusHandlerTests
{
    [TestFixture]
    public class When_called_and_update_duplicated : UpdateCertificatesPrintStatusHandlerTestsBase
    {
        private CertificatePrintStatusUpdateHandlerTestsFixture _fixture;
        private ValidationResponse _response;

        private const string CertificateReference = "00123456";
        private Guid CertificateId = Guid.NewGuid();

        private const int BatchNumber = 111;
        
        private static readonly DateTime StatusAt = DateTime.Parse("01/01/2021 12:00:00");
        private static readonly DateTime ChangedStatusAt = DateTime.Parse("01/01/2021 18:00:00");

        [SetUp]
        public void Arrange()
        {
            // Arrange
            _fixture = new CertificatePrintStatusUpdateHandlerTestsFixture()
                .WithCertificate(CertificateId, CertificateReference, CertificateStatus.NotDelivered, StatusAt, BatchNumber, StatusAt.AddMinutes(-5))
                .WithCertificateBatchLog(BatchNumber, CertificateReference, CertificateStatus.NotDelivered, StatusAt, "Original Reason", StatusAt.AddMinutes(5))
                .WithBatchLog(BatchNumber);
        }

        [TestCaseSource(nameof(TestSource))]
        public async Task Then_validation_response_is_valid_true_for_status_duplication(string status, DateTime statusAt, string reasonForChange, bool updateCertificateStatus, bool updateCertificateBatchLog)
        {
            // Arrange
            var request = new CertificatePrintStatusUpdateRequest
            {
                BatchNumber = BatchNumber,
                CertificateReference = CertificateReference,
                Status = status,
                StatusAt = statusAt,
                ReasonForChange = reasonForChange
            };

            // Act
            _response = await _fixture.Handle(request);
            
            // Assert
            _response.IsValid.Should().Be(true);
            _response.Errors.Count.Should().Be(0);
        }

        [TestCaseSource(nameof(TestSource))]
        public async Task Then_repository_update_print_status_is_called_for_printed(string status, DateTime statusAt, string reasonForChange, bool updateCertificateStatus, bool updateCertificateBatchLog)
        {
            // Arrange
            var request = new CertificatePrintStatusUpdateRequest
            {
                BatchNumber = BatchNumber,
                CertificateReference = CertificateReference,
                Status = status,
                StatusAt = statusAt,
                ReasonForChange = reasonForChange
            };

            // Act
            _response = await _fixture.Handle(request);

            // Assert
            if(updateCertificateBatchLog)
            {
                _fixture.VerifyUpdatePrintStatusCalled(CertificateId, BatchNumber, request.Status, request.StatusAt, request.ReasonForChange, updateCertificateStatus, updateCertificateBatchLog);
            }
            else
            {
                _fixture.VerifyUpdatePrintStatusNotCalled(CertificateId, BatchNumber);
            }
        }

        static IEnumerable<object[]> TestSource()
        {
            return new[]
            {
                new object[] { CertificateStatus.NotDelivered, StatusAt, "Original Reason", false, false },
                new object[] { CertificateStatus.Delivered, StatusAt, "Original Reason", false, true },
                new object[] { CertificateStatus.NotDelivered, ChangedStatusAt, "Original Reason", true, true },
                new object[] { CertificateStatus.NotDelivered, StatusAt, "Changed Reason", false, true }
            };
        }
    }
}
