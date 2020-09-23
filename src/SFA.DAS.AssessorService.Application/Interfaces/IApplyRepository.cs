using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AssessorService.ApplyTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IApplyRepository
    {
        Task<List<Domain.Entities.Apply>> GetCombindedApplications(Guid userId);
        Task<List<Domain.Entities.Apply>> GetOrganisationApplications(Guid userId);
        Task<List<Domain.Entities.Apply>> GetStandardApplications(Guid userId);
        Task<List<Domain.Entities.Apply>> GetOrganisationWithdrawalApplications(Guid userId);
        Task<List<Domain.Entities.Apply>> GetStandardWithdrawalApplications(Guid userId);
        
        Task<Domain.Entities.Apply> GetApplication(Guid applicationId);
        Task<Guid> CreateApplication(Domain.Entities.Apply apply);
        Task<bool> CanSubmitApplication(Guid applicationId);
        Task SubmitApplicationSequence(Domain.Entities.Apply apply);
        Task<int> GetNextAppReferenceSequence();
        Task<ApplicationReviewStatusCounts> GetApplicationReviewStatusCounts();
        Task<OrganisationApplicationsResult> GetOrganisationApplications(string reviewStatus, string sortColumn, int sortAscending, int pageSize, int pageIndex);
        Task<OrganisationApplicationsResult> GetOrganisationWithdrawalApplications(string reviewStatus, string sortColumn, int sortAscending, int pageSize, int pageIndex);
        Task<OrganisationApplicationsResult> GetStandardApplications(string organisationId, string reviewStatus, string sortColumn, int sortAscending, int pageSize, int pageIndex);
        Task UpdateGovernanceRecommendation(Guid id, GovernanceRecommendation governanceRecommendation);

        Task<List<FinancialApplicationSummaryItem>> GetOpenFinancialApplications();
        Task<List<FinancialApplicationSummaryItem>> GetFeedbackAddedFinancialApplications();
        Task<List<FinancialApplicationSummaryItem>> GetClosedFinancialApplications();
        Task StartFinancialReview(Guid id, string reviewer);
        Task ReturnFinancialReview(Guid id, FinancialGrade financialGrade);
        Task StartApplicationSectionReview(Guid id, int sequenceNo, int sectionNo, string reviewer);
        Task EvaluateApplicationSection(Guid id, int sequenceNo, int sectionNo, bool isSectionComplete, string evaluatedBy);
        Task ReturnApplicationSequence(Guid id, int sequenceNo, string sequenceStatus, string returnedBy);
        Task<bool> UpdateStandardData(Guid id, int standardCode, string referenceNumber, string standardName);
    }

    public class OrganisationApplicationsResult
    {
        public IEnumerable<ApplicationSummaryItem> PageOfResults { get; set; }
        public int TotalCount { get; set; }
    }
}
