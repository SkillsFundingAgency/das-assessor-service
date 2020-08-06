using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Staff
{
    public class StaffBatchLogResult
    {
        public int BatchNumber { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime SentToPrinterDate { get; set; }
        public int NumberOfCertificatesSent { get; set; }
        public DateTime? PrintedDate { get; set; }
        public int NumberOfCertificatesPrinted { get; set; }
    }
}
