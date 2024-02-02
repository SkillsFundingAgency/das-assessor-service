using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.QnA.Api.Types.Page;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IApplicationApiClient
    {
        Task<List<ApplicationResponse>> GetOrganisationApplications(Guid userId);
        Task<List<ApplicationResponse>> GetStandardApplications(Guid userId);
        Task<List<ApplicationResponse>> GetWithdrawalApplications(Guid userId);
        Task<List<ApplicationResponse>> GetOrganisationWithdrawalApplications(Guid userId);
        Task<List<ApplicationResponse>> GetStandardWithdrawalApplications(Guid userId);
        Task<ApplicationResponse> GetApplication(Guid id);
        Task<ApplicationResponse> GetApplicationForUser(Guid id, Guid userId);
        Task<DateTime?> GetLatestWithdrawalDateForStandard(Guid organisationId, int? standardCode);

        Task<Guid> CreateApplication(CreateApplicationRequest createApplicationRequest);
        Task DeleteApplications(DeleteApplicationsRequest deleteApplicationsRequest);
        Task<bool> SubmitApplicationSequence(SubmitApplicationSequenceRequest submitApplicationRequest);

        Task<bool> UpdateStandardData(Guid Id, int standardCode, string referenceNumber, string standardName, List<string> versions, string standardApplicationType = null);
        Task<bool> ResetApplicationToStage1(Guid applicationId);

        #region Application

        Task<ApplicationReviewStatusCounts> GetApplicationReviewStatusCounts();

        Task<PaginatedList<ApplicationSummaryItem>> GetOrganisationApplications(OrganisationApplicationsRequest organisationApplicationsRequest);

        Task<PaginatedList<ApplicationSummaryItem>> GetStandardApplications(StandardApplicationsRequest standardApplicationsRequest);

        Task<PaginatedList<ApplicationSummaryItem>> GetWithdrawalApplications(WithdrawalApplicationsRequest withdrawalApplicationsRequest);

        Task StartApplicationSectionReview(Guid applicationId, int sequenceNo, int sectionNo, string reviewer);

        Task EvaluateSection(Guid applicationId, int sequenceNo, int sectionNo, bool isSectionComplete, string evaluatedBy);

        Task ReturnApplicationSequence(Guid applicationId, int sequenceNo, string returnType, string returnedBy);

        #endregion

        #region Financial

        Task<List<FinancialApplicationSummaryItem>> GetOpenFinancialApplications();

        Task<List<FinancialApplicationSummaryItem>> GetFeedbackAddedFinancialApplications();

        Task<List<FinancialApplicationSummaryItem>> GetClosedFinancialApplications();

        Task StartFinancialReview(Guid applicationId, string reviewer);

        Task ReturnFinancialReview(Guid applicationId, Domain.Entities.FinancialGrade grade);

        #endregion

        #region Feedback

        Task AddFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, Feedback feedback);

        Task DeleteFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, Guid feedbackId);

        #endregion

        #region Answer Injection Service

        Task UpdateFinancials(UpdateFinancialsRequest updateFinancialsRequest);

        #endregion
    }
}
