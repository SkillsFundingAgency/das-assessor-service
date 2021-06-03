using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IApplyRepository
    {
        Task<Domain.Entities.Apply> GetApply(Guid applicationId);
        Task<ApplySummary> GetApplication(Guid applicationId, Guid? userId);
        Task<List<ApplySummary>> GetOrganisationApplications(Guid userId);
        Task<List<ApplySummary>> GetStandardApplications(Guid userId);
        Task<List<ApplySummary>> GetWithdrawalApplications(Guid userId);
        Task<List<ApplySummary>> GetOrganisationWithdrawalApplications(Guid userId);
        Task<List<ApplySummary>> GetStandardWithdrawalApplications(Guid userId);
        
        Task<Guid> CreateApplication(Domain.Entities.Apply apply);
        Task<bool> CanSubmitApplication(Guid applicationId);
        Task SubmitApplicationSequence(Domain.Entities.Apply apply);
        Task<int> GetNextAppReferenceSequence();
        Task<ApplicationReviewStatusCounts> GetApplicationReviewStatusCounts();
        Task<ApplicationsResult> GetOrganisationApplications(string reviewStatus, string sortColumn, int sortAscending, int pageSize, int pageIndex);
        Task<ApplicationsResult> GetStandardApplications(string organisationId, string reviewStatus, string sortColumn, int sortAscending, int pageSize, int pageIndex);
        Task<ApplicationsResult> GetWithdrawalApplications(string organisationId, string reviewStatus, string sortColumn, int sortAscending, int pageSize, int pageIndex);
        Task<List<FinancialApplicationSummaryItem>> GetOpenFinancialApplications();
        Task<List<FinancialApplicationSummaryItem>> GetFeedbackAddedFinancialApplications();
        Task<List<FinancialApplicationSummaryItem>> GetClosedFinancialApplications();
        Task StartFinancialReview(Guid id, string reviewer);
        Task ReturnFinancialReview(Guid id, FinancialGrade financialGrade);
        Task StartApplicationSectionReview(Guid id, int sequenceNo, int sectionNo, string reviewer);
        Task EvaluateApplicationSection(Guid id, int sequenceNo, int sectionNo, bool isSectionComplete, string evaluatedBy);
        Task ReturnApplicationSequence(Guid id, int sequenceNo, string sequenceStatus, string returnedBy);
        Task<bool> UpdateStandardData(Guid id, int standardCode, string referenceNumber, string standardName, List<string> versions, string applicationType);
        Task<bool> ResetApplicatonToStage1(Guid id, Guid userId);
    }

    public class ApplicationsResult
    {
        public IEnumerable<ApplicationSummaryItem> PageOfResults { get; set; }
        public int TotalCount { get; set; }
    }
}
