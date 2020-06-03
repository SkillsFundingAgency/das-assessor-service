using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class UpdateBatchLogSentToPrinterRequest : IRequest
    {
        public int BatchNumber { get; set; }
        public List<string> CertificateReferences { get; set; }
    }
}
