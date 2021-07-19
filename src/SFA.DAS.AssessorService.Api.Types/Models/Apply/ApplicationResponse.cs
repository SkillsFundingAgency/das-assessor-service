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
        public string ApplicationType { get; set; }
        public string StandardApplicationType { get; set; }
        public Guid OrganisationId { get; set; }
        public string EndPointAssessorName { get; set; }
        public FinancialGrade FinancialGrade { get; set; }
        public string ApplicationStatus { get; set; }
        public string ReviewStatus { get; set; }
        public string FinancialReviewStatus { get; set; }
        public ApplyData ApplyData { get; set; }
        public int? StandardCode { get; set; }
        public string StandardReference { get; set; }
        public string CreatedBy { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }

        public bool IsInitialWithFinancialHealthChecks => ApplicationType == ApplicationTypes.InitialWithFinancialHealthChecks;

        public bool IsInitialWithoutFinancialHealthChecks => ApplicationType == ApplicationTypes.InitialWithoutFinancialHealthChecks;

        public bool IsInitialApplication => IsInitialWithFinancialHealthChecks || IsInitialWithoutFinancialHealthChecks;

        public bool IsAdditionalStandardWithFinancialHealthChecks => ApplicationType == ApplicationTypes.AdditionalStandardWithFinancialHealthChecks;

        public bool IsAdditionalStandardWithoutFinancialHealthChecks => ApplicationType == ApplicationTypes.AdditionalStandardWithoutFinancialHealthChecks;

        public bool IsAdditionalStandardApplication => IsAdditionalStandardWithFinancialHealthChecks || IsAdditionalStandardWithoutFinancialHealthChecks;

        public bool IsOrganisationApplication => IsInitialApplication || IsAdditionalStandardWithFinancialHealthChecks;

        public bool IsStandardApplication => ApplicationType == ApplicationTypes.Standard || IsAdditionalStandardApplication;

        public bool IsStandardWithdrawalApplication => ApplicationType == ApplicationTypes.StandardWithdrawal;

        public bool IsOrganisationWithdrawalApplication => ApplicationType == ApplicationTypes.OrganisationWithdrawal;
        
        public bool IsWithdrawalApplication => IsStandardWithdrawalApplication || IsOrganisationWithdrawalApplication;
    }
}
