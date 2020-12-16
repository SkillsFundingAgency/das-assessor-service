using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using System;
using System.Linq;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class ApplicationResponse 
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid OrganisationId { get; set; }
        public string EndPointAssessorName { get; set; }
        public FinancialGrade financialGrade { get; set; }
        public string ApplicationStatus { get; set; }
        public string ReviewStatus { get; set; }
        public string FinancialReviewStatus { get; set; }
        public ApplyData ApplyData { get; set; }
        public int? StandardCode { get; set; }
        public string CreatedBy { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }

        public string GetApplicationType()
        {
            if (IsSequenceRequired(ApplyConst.ORGANISATION_SEQUENCE_NO) && IsSequenceRequired(ApplyConst.STANDARD_SEQUENCE_NO))
            {
                return ApplicationTypes.Combined;
            }
            else if (IsSequenceRequired(ApplyConst.STANDARD_SEQUENCE_NO))
            {
                return ApplicationTypes.Standard;
            }
            else if(IsSequenceRequired(ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO))
            {
                return ApplicationTypes.OrganisationWithdrawal;
            }
            else if (IsSequenceRequired(ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO))
            {
                return ApplicationTypes.StandardWithdrawal;
            }

            return string.Empty;
        }

        public bool IsSequenceRequired(int sequenceNo)
        {
            // a sequence cannot be considered required if it does not exist in the ApplyData
            return ApplyData?.Sequences?.Any(x => x.SequenceNo == sequenceNo && !x.NotRequired) ?? false;
        }

        public bool IsSequenceActive(int sequenceNo)
        {
            // a sequence can be considered active even if it does not exist in the ApplyData, since it has not yet been submitted and is in progress.
            return ApplyData?.Sequences?.Any(x => x.SequenceNo == sequenceNo && x.IsActive) ?? true;
        }
    }
}
