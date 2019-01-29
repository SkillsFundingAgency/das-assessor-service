using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class BatchLog
    {
        public Guid Id { get; set; }
        public string Period { get; set; }
        public DateTime BatchCreated { get; set; }
        public DateTime ScheduledDate { get; set; }
        public int BatchNumber { get; set; }
        public int NumberOfCertificates { get; set; }
        public int NumberOfCoverLetters { get; set; }
        public string CertificatesFileName { get; set; }      
        public DateTime FileUploadStartTime { get; set; }
        public DateTime FileUploadEndTime { get; set; }
        public string BatchData { get; set; }

    }
}
