using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class Apply: BaseEntity
    {
        public Guid Id { get; set; }
        public string StandardApplicationType { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }
        public string ApplicationStatus { get; set; }
        public string FinancialReviewStatus { get; set; }
        public string ReviewStatus { get; set; }
        public FinancialGrade FinancialGrade { get; set; }
        public int? StandardCode { get; set; }
        public ApplyData ApplyData { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string DeletedBy { get; set; }
        public string StandardReference { get; set; }
        public bool ApplyViaOptIn { get; set; }
    }

    // Entity used by EF in AssessorDbContext - flattens applydata and financialgrade
    public class ApplyEF : BaseEntity
    {
        public Guid Id { get; set; }
        public string StandardApplicationType { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }
        public string ApplicationStatus { get; set; }
        public string FinancialReviewStatus { get; set; }
        public string ReviewStatus { get; set; }
        public string FinancialGrade { get; set; }
        public int? StandardCode { get; set; }
        public string ApplyData { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string DeletedBy { get; set; }
        public string StandardReference { get; set; }
    }

    public class ApplyData
    {
        public List<ApplySequence> Sequences { get; set; }
        public ApplyInfo Apply { get; set; }

        [JsonIgnore]
        public int[] RequiredSequences => Sequences.Where(seq => !seq.NotRequired).Select(seq => seq.SequenceNo).ToArray();
    }

    public class ApplyInfo
    {
        public string ReferenceNumber { get; set; }

        public int? StandardCode { get; set; }
        public string StandardReference { get; set; }
        public string StandardName { get; set; }
        public List<string> Versions { get; set; }

        public List<Submission> InitSubmissions { get; private set; } = new List<Submission>();

        public void AddInitSubmission(Submission submission)
        {
            if (InitSubmissions == null)
                ResetInitSubmissions();

            InitSubmissions.Add(submission);
        }

        public void ResetInitSubmissions()
        {
            InitSubmissions = new List<Submission>();
        }

        [JsonIgnore]
        public Submission LatestInitSubmission => InitSubmissions?.OrderByDescending(o => o.SubmittedAt).FirstOrDefault();

        public int InitSubmissionsCount => InitSubmissions?.Count ?? 0;
        public DateTime? LatestInitSubmissionDate => LatestInitSubmission?.SubmittedAt;
        public DateTime? InitSubmissionFeedbackAddedDate { get; set; }
        public DateTime? InitSubmissionClosedDate { get; set; }

        public List<Submission> StandardSubmissions { get; private set; } = new List<Submission>();

        public void AddStandardSubmission(Submission submission)
        {
            if (StandardSubmissions == null)
                ResetStandardSubmissions();

            StandardSubmissions.Add(submission);
        }

        public void ResetStandardSubmissions()
        {
            StandardSubmissions = new List<Submission>();
        }

        [JsonIgnore]
        public Submission LatestStandardSubmission => StandardSubmissions?.OrderByDescending(o => o.SubmittedAt).FirstOrDefault();

        public int StandardSubmissionsCount => StandardSubmissions?.Count ?? 0;
        public DateTime? LatestStandardSubmissionDate => LatestStandardSubmission?.SubmittedAt;
        public DateTime? StandardSubmissionFeedbackAddedDate { get; set; }
        public DateTime? StandardSubmissionClosedDate { get; set; }

        public List<Submission> OrganisationWithdrawalSubmissions { get; private set; } = new List<Submission>();

        public void AddOrganisationWithdrawalSubmission(Submission submission)
        {
            if (OrganisationWithdrawalSubmissions == null)
                ResetOrganisationWithdrawalSubmissions();

            OrganisationWithdrawalSubmissions.Add(submission);
        }

        public void ResetOrganisationWithdrawalSubmissions()
        {
            OrganisationWithdrawalSubmissions = new List<Submission>();
        }

        [JsonIgnore]
        public Submission LatestOrganisationWithdrawalSubmission => OrganisationWithdrawalSubmissions?.OrderByDescending(o => o.SubmittedAt).FirstOrDefault();

        public int OrganisationWithdrawalSubmissionsCount => OrganisationWithdrawalSubmissions?.Count ?? 0;
        public DateTime? LatestOrganisationWithdrawalSubmissionDate => LatestOrganisationWithdrawalSubmission?.SubmittedAt;
        public DateTime? OrganisationWithdrawalSubmissionFeedbackAddedDate { get; set; }
        public DateTime? OrganisationWithdrawalSubmissionClosedDate { get; set; }

        public List<Submission> StandardWithdrawalSubmissions { get; private set; } = new List<Submission>();

        public void AddStandardWithdrawalSubmission(Submission submission)
        {
            if (StandardWithdrawalSubmissions == null)
                ResetStandardWithdrawalSubmissions();

            StandardWithdrawalSubmissions.Add(submission);
        }

        public void ResetStandardWithdrawalSubmissions()
        {
            StandardWithdrawalSubmissions = new List<Submission>();
        }

        [JsonIgnore]
        public Submission LatestStandardWithdrawalSubmission => StandardWithdrawalSubmissions?.OrderByDescending(o => o.SubmittedAt).FirstOrDefault();

        public int StandardWithdrawalSubmissionsCount => StandardWithdrawalSubmissions?.Count ?? 0;
        public DateTime? LatestStandardWithdrawalSubmissionDate => LatestStandardWithdrawalSubmission?.SubmittedAt;
        public DateTime? StandardWithdrawalSubmissionFeedbackAddedDate { get; set; }
        public DateTime? StandardWithdrawalSubmissionClosedDate { get; set; }

        [JsonIgnore]
        public string StandardWithReference => $"{StandardName} ({StandardReference})";
    }

    public class Submission
    {
        public DateTime SubmittedAt { get; set; }
        public Guid SubmittedBy { get; set; }
        public string SubmittedByEmail { get; set; }
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
}
