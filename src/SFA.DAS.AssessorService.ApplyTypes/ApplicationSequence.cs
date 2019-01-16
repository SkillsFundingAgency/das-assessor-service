using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.ApplyTypes
{
    public class ApplicationSequence : ApplyTypeBase
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public int SequenceId { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public SequenceData SequenceData { get; set; }
        public List<ApplicationSection> Sections { get; set; }
    }

    public class SequenceData
    {
        public List<Feedback> Feedback { get; set; }
    }

    public class ApplicationSequenceStatus
    {
        public const string Draft = "Draft";
        public const string Submitted = "Submitted";
        public const string InProgress = "In Progress";
        public const string FeedbackAdded = "FeedbackAdded";
        public const string Rejected = "Rejected";
        public const string Approved = "Approved";
    }

    public enum SequenceId
    {
        Stage1 = 1,
        Stage2 = 2
    }
}
