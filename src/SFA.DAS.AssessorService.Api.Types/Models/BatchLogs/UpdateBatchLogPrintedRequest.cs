using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.BatchLogs
{
    public class UpdateBatchLogPrintedRequest : IRequest<ValidationResponse>
    {
        public int BatchNumber { get; set; }
        public DateTime? BatchDate { get; set; }
        public int? PostalContactCount { get; set; }
        public int? TotalCertificateCount { get; set; }
        public DateTime? PrintedDate { get; set; }
        public DateTime? DateOfResponse { get; set; }
    }
}
