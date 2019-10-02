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
        Task<List<FinancialApplicationSummaryItem>> GetOpenFinancialApplications();
        Task<List<FinancialApplicationSummaryItem>> GetFeedbackAddedFinancialApplications();
        Task<List<FinancialApplicationSummaryItem>> GetClosedFinancialApplications();
        Task StartFinancialReview(Guid id);
        Task UpdateApplicationFinancialGrade(Guid id,FinancialGrade financialGrade);
        Task UpdateApplicationSectionStatus(Guid id, string sequenceNo, string sectionNo, string status);
        Task UpdateInitialStandardData(UpdateInitialStandardDataRequest standardRequest);
    }
}
