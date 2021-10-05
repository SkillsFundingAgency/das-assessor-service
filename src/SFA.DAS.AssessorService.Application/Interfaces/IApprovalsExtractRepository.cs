using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IApprovalsExtractRepository
    {
        Task<DateTime?> GetLatestExtractTimestamp();
        Task UpsertApprovalsExtract(List<ApprovalsExtract> approvalsExtract);
        Task<int> PopulateLearner();
    }
}
