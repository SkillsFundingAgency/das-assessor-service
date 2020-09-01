using System;

namespace SFA.DAS.AssessorService.Domain.DTOs.Staff
{
    public class BatchLogSummary
    {
        public int BatchNumber { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime SentToPrinterAt { get; set; }
        public int NumberOfCertificatesSent { get; set; }
        public DateTime? PrintedAt { get; set; }
        public int NumberOfCertificatesPrinted { get; set; }
    }
}
