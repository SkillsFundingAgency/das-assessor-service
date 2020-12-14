using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.BatchLogs
{
    public class UpdateBatchLogSentToPrinterRequest : IRequest<ValidationResponse>
    {
        public int BatchNumber { get; set; }
        public DateTime BatchCreated { get; set; }
        public int NumberOfCertificates { get; set; }
        public int NumberOfCoverLetters { get; set; }
        public string CertificatesFileName { get; set; }
        public DateTime FileUploadStartTime { get; set; }
        public DateTime FileUploadEndTime { get; set; }
    }
}
