using System;
using SFA.DAS.AssessorService.Domain.JsonData.Printing;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class BatchLogResponse
    {
        public Guid? Id { get; set; }
        public DateTime BatchCreated { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string Period { get; set; }
        public int BatchNumber { get; set; }
        public int NumberOfCertificates { get; set; }
        public int NumberOfCoverLetters { get; set; }
        public string CertificatesFileName { get; set; }
        public DateTime FileUploadStartTime { get; set; }
        public DateTime FileUploadEndTime { get; set; }
        public BatchDetails BatchData { get; set; }
    }
}
