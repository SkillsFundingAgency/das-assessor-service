using SFA.DAS.AssessorService.Application.Infrastructure.OuterApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IStandardImportService
    {
        Task DeleteAllStandards();
        Task LoadStandards(IEnumerable<StandardDetailResponse> standards);
        Task UpsertStandardCollations(IEnumerable<StandardDetailResponse> standards);
        Task UpsertStandardNonApprovedCollations(IEnumerable<StandardDetailResponse> standards);
    }
}