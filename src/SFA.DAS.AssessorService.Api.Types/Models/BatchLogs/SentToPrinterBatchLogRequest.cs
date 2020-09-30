using System.Collections.Generic;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class SentToPrinterBatchLogRequest : IRequest<ValidationResponse>
    {
        public int BatchNumber { get; set; }
        public List<string> CertificateReferences { get; set; }
    }
}
