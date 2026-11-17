using System;

namespace SFA.DAS.AssessorService.Messages.Events
{
    public class OutcomeRecordedEvent
    {
        public long ApprenticeshipId { get; set; }
        public DateTime AchievementDate { get; set; }
        public OutcomeType Outcome { get; set; }
        public string StandardName { get; set; }
        public Guid CertificateId { get; set; }
    }
}
