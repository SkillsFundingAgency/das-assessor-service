using SFA.DAS.AssessorService.Application.Infrastructure.OuterApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IStandardImportService
    {
        Task DeleteAllStandardsAndOptions();
        Task LoadStandards(IEnumerable<StandardDetailResponse> standards);
        Task LoadOptions(IEnumerable<StandardDetailResponse> standards);
    }
}