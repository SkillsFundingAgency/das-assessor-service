using System;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class UpdateCertificatePrintRequestCommand : IRequest<Unit>
    {
        public Guid CertificateId { get; set; }

        public CertificatePrintAddress Address { get; set; }

        public DateTime PrintRequestedAt { get; set; }

        public string PrintRequestedBy { get; set; }
    }
}
