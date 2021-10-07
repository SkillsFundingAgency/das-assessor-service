using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IApprovalsExtractRepository
    {
        Task<DateTime?> GetLatestExtractTimestamp();
        Task ClearApprovalsExtractStaging();
        Task UpsertApprovalsExtractToStaging(List<ApprovalsExtract> approvalsExtract);
        Task<int> PopulateLearner();
        Task PopulateApprovalsExtract();
        Task UpsertProvidersFromApprovalsExtract();
        Task UpsertProvidersFromLearners();
    }
}
