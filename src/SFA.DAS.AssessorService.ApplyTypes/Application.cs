using Newtonsoft.Json;
using SFA.DAS.AssessorService.ApplyTypes.CharityCommission;
using SFA.DAS.AssessorService.ApplyTypes.CompaniesHouse;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.ApplyTypes
{
    public class ApplicationData
    {
        public string OrganisationReferenceId { get; set; }
        public string OrganisationName { get; set; }
        public string ReferenceNumber { get; set; }
        public string StandardName { get; set; }
        public string OrganisationType { get; set; }
        public int? StandardCode { get; set; }
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

    public class ApplyData
    {
        public List<ApplySequence> Sequences { get; set; }
        public Apply Apply { get; set; }

        [JsonIgnore]
        public int[] RequiredSequences => Sequences.Where(seq => !seq.NotRequired).Select(seq => seq.SequenceNo).ToArray();
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
        public bool NotRequired { get; set; }
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
        public int? StandardCode { get; set; }
        public string StandardReference { get; set; }
        public string StandardName { get; set; }

        public List<Submission> InitSubmissions { get; set; } = new List<Submission>();

        [JsonIgnore]
        public Submission LatestInitSubmission => InitSubmissions?.OrderByDescending(o => o.SubmittedAt).FirstOrDefault();

        public int InitSubmissionsCount => InitSubmissions?.Count ?? 0;
        public DateTime? LatestInitSubmissionDate => LatestInitSubmission?.SubmittedAt;
        public DateTime? InitSubmissionFeedbackAddedDate { get; set; }
        public DateTime? InitSubmissionClosedDate { get; set; }

        public List<Submission> StandardSubmissions { get; set; } = new List<Submission>();

        [JsonIgnore]
        public Submission LatestStandardSubmission => StandardSubmissions?.OrderByDescending(o => o.SubmittedAt).FirstOrDefault();

        public int StandardSubmissionsCount => StandardSubmissions?.Count ?? 0;
        public DateTime? LatestStandardSubmissionDate => LatestStandardSubmission?.SubmittedAt;
        public DateTime? StandardSubmissionFeedbackAddedDate { get; set; }
        public DateTime? StandardSubmissionClosedDate { get; set; }

        public List<Submission> OrganisationWithdrawalSubmissions { get; set; } = new List<Submission>();
        
        [JsonIgnore]
        public Submission LatestOrganisationWithdrawalSubmission => OrganisationWithdrawalSubmissions?.OrderByDescending(o => o.SubmittedAt).FirstOrDefault();

        public int OrganisationWithdrawalSubmissionsCount => OrganisationWithdrawalSubmissions?.Count ?? 0;
        public DateTime? LatestOrganisationWithdrawalSubmissionDate => LatestOrganisationWithdrawalSubmission?.SubmittedAt;
        public DateTime? OrganisationWithdrawalSubmissionFeedbackAddedDate { get; set; }
        public DateTime? OrganisationWithdrawalSubmissionClosedDate { get; set; }

        public List<Submission> StandardWithdrawalSubmissions { get; set; } = new List<Submission>();
        
        [JsonIgnore]
        public Submission LatestStandardWithdrawalSubmission => StandardWithdrawalSubmissions?.OrderByDescending(o => o.SubmittedAt).FirstOrDefault();

        public int StandardWithdrawalSubmissionsCount => StandardWithdrawalSubmissions?.Count ?? 0;
        public DateTime? LatestStandardWithdrawalSubmissionDate => LatestStandardWithdrawalSubmission?.SubmittedAt;
        public DateTime? StandardWithdrawalSubmissionFeedbackAddedDate { get; set; }
        public DateTime? StandardWithdrawalSubmissionClosedDate { get; set; }
    }

    public class Submission
    {
        public DateTime SubmittedAt { get; set; }
        public Guid SubmittedBy { get; set; }
        public string SubmittedByEmail { get; set; }
    }
}
