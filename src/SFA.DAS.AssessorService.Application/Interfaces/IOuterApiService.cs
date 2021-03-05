using SFA.DAS.AssessorService.Application.Infrastructure.OuterApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IOuterApiService
    {
        Task<IEnumerable<GetStandardsListItem>> GetAllStandards();
    }
}
