using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.ExternalApis.IFAStandards.Types;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IStandardRepository
    {
        Task<string> UpsertStandards(List<StandardCollation> standards);

        Task<List<StandardCollation>> GetStandardCollations();
    }
}
