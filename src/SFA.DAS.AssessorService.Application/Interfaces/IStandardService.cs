using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IStandardService
    {
        Task<IEnumerable<StandardCollation>> GetAllStandards();

        Task<StandardCollation> GetStandard(int standardId);
        Task<StandardCollation> GetStandard(string referenceNumber);        
        
        Task<IEnumerable<StandardCollation>> GatherAllStandardDetails();
        
    }
}