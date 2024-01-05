using System.Collections.Generic;
using System;

namespace SFA.DAS.AssessorService.ApplyTypes
{
    public class ApplicationSequence : ApplyTypeBase
    {
        public Guid ApplicationId { get; set; }
        public int SequenceId { get; set; }
        public bool IsActive { get; set; }
        public List<ApplicationSection> Sections { get; set; }
        public bool NotRequired { get; set; }
    }

    public static class ApplicationSequenceStatus
    {
        public const string Draft = "Draft";
        public const string Submitted = "Submitted";
        public const string InProgress = "In Progress";
        public const string FeedbackAdded = "FeedbackAdded";
        public const string Resubmitted = "Resubmitted";
        public const string Declined = "Declined";
        public const string Approved = "Approved";
    }
}
