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
        Task<IEnumerable<Standard>> GetStandardVersionsByLarsCode(int larsCode);
        /// <summary>
        /// Method can take LarsCode, IFateReferenceNumber or StandardUId and will return a standard.
        /// If LarsCode or IFateReferenceNumber is supplied, One Standard, the latest version will 
        /// be returned, based on highest version number unless optional parameter version is also supplied.
        /// </summary>
        /// <param name="id">LarsCode, IFateReferenceNumber or StandardUId</param>
        /// <param name="version">Optional parameter version, taken into account if LarsCode or IFateReference Supplied</param>
        /// <returns></returns>
        Task<Standard> GetStandardVersionById(string id, string version = null);
        Task<IEnumerable<StandardOptions>> GetAllStandardOptions();
        Task<IEnumerable<StandardOptions>> GetStandardOptionsForLatestStandardVersions();
        Task<StandardOptions> GetStandardOptionsByStandardId(string id);
        Task<StandardOptions> GetStandardOptionsByStandardIdAndVersion(string id, string version);
        Task<IEnumerable<StandardVersion>> GetEPAORegisteredStandardVersions(string endPointAssessorOrganisationId, int? larsCode = null);
    }
}