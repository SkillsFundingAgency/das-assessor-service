using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.ApplyTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IApplyRepository
    {
        Task<List<Domain.Entities.Apply>> GetUserApplications(Guid userId);
        Task<List<Domain.Entities.Apply>> GetOrganisationApplications(Guid userId);
        Task<Domain.Entities.Apply> GetApplication(Guid applicationId);
        Task<Guid> CreateApplication(Domain.Entities.Apply apply);
        Task SubmitApplicationSequence(Domain.Entities.Apply apply);
        Task<int> GetNextAppReferenceSequence();
        Task<List<ApplicationSummaryItem>> GetOpenApplications(int sequenceNo);
        Task<List<ApplicationSummaryItem>> GetFeedbackAddedApplications();
        Task<List<ApplicationSummaryItem>> GetClosedApplications();
        Task<List<FinancialApplicationSummaryItem>> GetOpenFinancialApplications();
        Task<List<FinancialApplicationSummaryItem>> GetFeedbackAddedFinancialApplications();
        Task<List<FinancialApplicationSummaryItem>> GetClosedFinancialApplications();
        Task StartFinancialReview(Guid id, string reviewer);
        Task ReturnFinancialReview(Guid id, FinancialGrade financialGrade);
        Task StartApplicationSectionReview(Guid id, int sequenceNo, int sectionNo, string reviewer);
        Task EvaluateApplicationSection(Guid id, int sequenceNo, int sectionNo, bool isSectionComplete, string evaluatedBy);
        Task ReturnApplicationSequence(Guid id, int sequenceNo, string sequenceStatus, string returnedBy);
        Task UpdateInitialStandardData(UpdateInitialStandardDataRequest standardRequest);
    }
}
