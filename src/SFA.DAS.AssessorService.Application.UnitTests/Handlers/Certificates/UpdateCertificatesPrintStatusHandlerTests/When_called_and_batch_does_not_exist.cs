using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Domain.Consts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.UpdateCertificatesPrintStatusHandlerTests
{
    public class When_called_and_batch_does_not_exist : UpdateCertificatesPrintStatusHandlerTestsBase
    {
        private ValidationResponse _response;

        [SetUp]
        public async Task Arrange()
        {
            base.BaseArrange();
            
            _response = await _sut.Handle(new UpdateCertificatesPrintStatusRequest
            {
                CertificatePrintStatuses = new List<CertificatePrintStatus>
                {
                    new CertificatePrintStatus
                    {
                        BatchNumber = _batchNumber + 999,
                        CertificateReference = _certificateReference1,
                        Status = CertificateStatus.Delivered,
                        StatusChangedAt = DateTime.UtcNow
                    }
                }
            }, new CancellationToken());
        }

        [Test]
        public void Then_validation_response_is_valid_false()
        {
            _response.IsValid.Should().Be(false);
            _response.Errors.Count.Should().Be(1);
            _response.Errors[0].Field.Should().Be("CertificatePrintStatuses");
            _response.Errors[0].ErrorMessage.Contains("BatchNumber");
        }
    }
}
