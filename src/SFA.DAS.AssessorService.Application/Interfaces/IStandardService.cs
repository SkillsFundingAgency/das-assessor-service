using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IStandardService
    {
        Task<IEnumerable<StandardCollation>> GetAllStandards();

        Task<StandardCollation> GetStandard(int standardId);
        Task<StandardCollation> GetStandard(string referenceNumber);

        Task<IEnumerable<EPORegisteredStandards>> GetEpaoRegisteredStandards(string endPointAssessorOrganisationId);

        // New Standard Versioning Methods
        Task<IEnumerable<Standard>> GetAllStandardVersions();
        Task<IEnumerable<Standard>> GetStandardVersions(int standardId);
        Task<IEnumerable<StandardOptions>> GetStandardOptions();
        Task<StandardOptions> GetStandardOptionsByStandardId(string id);
        Task<StandardOptions> GetStandardOptionsByStandardReferenceAndVersion(string standardReference, string version);
    }
}