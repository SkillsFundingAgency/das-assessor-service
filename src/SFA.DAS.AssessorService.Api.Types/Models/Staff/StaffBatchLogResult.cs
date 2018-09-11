using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Staff
{
    public class StaffBatchLogResult
    {
        public int BatchNumber { get; set; }
        public DateTime ScheduledDate { get; set; }
        public int NumberOfCertificates { get; set; }
        public int NumberOfCoverLetters { get; set; }
    }
}
