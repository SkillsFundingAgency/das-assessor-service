using SFA.DAS.AssessorService.ApplyTypes.CharityCommission;
using SFA.DAS.AssessorService.ApplyTypes.CompaniesHouse;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.ApplyTypes
{
    public class ApplicationData
    {
        public string OrganisationReferenceId { get; set; }
        public string OrganisationName { get; set; }
        public string ReferenceNumber { get; set; }
        public string StandardName { get; set; }
        public string StandardCode { get; set; }
        public string TradingName { get; set; }
        public bool UseTradingName { get; set; }
        public string ContactGivenName { get; set; }

        public CompaniesHouseSummary CompanySummary { get; set; }
        public CharityCommissionSummary CharitySummary { get; set; }
    }

    public class StandardApplicationData
    {
        public string StandardName { get; set; }
        public int StandardCode { get; set; }
        public Guid UserId { get; set; }
        public string UserEmail { get; set; }
    }

    public class Submission
    {
        public DateTime SubmittedAt { get; set; }
        public Guid SubmittedBy { get; set; }
        public string SubmittedByEmail { get; set; }
    }
    

    public class ApplyData
    {
        public List<ApplySequence> Sequences { get; set; }
        public Apply Apply { get; set; }
    }


    public class ApplySequence
    {
        public Guid SequenceId { get; set; }
        public List<ApplySection> Sections { get; set; }
        public string Status { get; set; }
        public int SequenceNo { get; set; }
        public bool IsActive { get; set; }
        public bool NotRequired { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string ApprovedBy { get; set; }

    }

    public class ApplySection
    {
        public Guid SectionId { get; set; }
        public int SectionNo { get; set; }
        public string Status { get; set; }
        public DateTime? ReviewStartDate { get; set; }
        public string ReviewedBy { get; set; }
        public DateTime? EvaluatedDate { get; set; }
        public string EvaluatedBy { get; set; }
        public bool? RequestedFeedbackAnswered { get; set; }
    }

    public class Feedback
    {
        public DateTime? Feedbackdate { get; set; }
        public string FeedbackBy { get; set; }
        public bool FeedbackAnswered { get; set; }
        public DateTime? FeedbackAnsweredDate { get; set; }
        public string FeedbackAnsweredBy { get; set; }
    }

    public class Apply
    {
        public string ReferenceNumber { get; set; }
        public int StandardCode { get; set; }
        public string StandardReference { get; set; }
        public string StandardName { get; set; }
        public List<InitSubmission> InitSubmissions { get; set; }
        public List<StandardSubmission> StandardSubmissions { get; set; }
        public int InitSubmissionCount { get; set; }
        public DateTime? LatestInitSubmissionDate { get; set; }
        public DateTime? InitSubmissionFeedbackAddedDate { get; set; }
        public DateTime? InitSubmissionClosedDate { get; set; }
        public int StandardSubmissionsCount { get; set; }
        public DateTime? LatestStandardSubmissionDate { get; set; }
        public DateTime? StandardSubmissionFeedbackAddedDate { get; set; }
        public DateTime? StandardSubmissionClosedDate { get; set; }
    }

    public class InitSubmission
    {
        public DateTime SubmittedAt { get; set; }
        public Guid SubmittedBy { get; set; }
        public string SubmittedByEmail { get; set; }
    }

    public class StandardSubmission
    {
        public DateTime SubmittedAt { get; set; }
        public Guid SubmittedBy { get; set; }
        public string SubmittedByEmail { get; set; }
    }

}
