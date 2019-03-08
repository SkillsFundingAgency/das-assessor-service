using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IStandardService
    {
        Task<IEnumerable<StandardSummary>> GetAllStandardsV2();
        Task<IEnumerable<Standard>> GetAllStandards();
        Task<IEnumerable<StandardCollation>> GatherAllStandardDetails();
        Task<Standard> GetStandard(int standardId);
        Task<IEnumerable<StandardSummary>> GetAllStandardSummaries();

    }
}