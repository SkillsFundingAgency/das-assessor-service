using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class UpdateCertificatesBatchToIndicatePrintedRequest : IRequest
    {
        public int BatchNumber { get; set; }
        public List<CertificateStatus> CertificateStatuses { get; set; }
    }
}