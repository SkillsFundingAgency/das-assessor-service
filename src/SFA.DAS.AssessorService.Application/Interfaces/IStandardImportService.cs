using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.OuterApi;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IStandardImportService
    {
        Task PrepareImport();
        Task StageStandards(IEnumerable<StandardDetailResponse> standards);
        Task StageOptions(IEnumerable<StandardDetailResponse> standards);
        Task MergeStandardsFromStaging();
    }
}