using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IStandardVersionApiClient
    {
        Task<IEnumerable<StandardVersion>> GetAllStandardVersions();
        Task<IEnumerable<StandardVersion>> GetLatestStandardVersions();
        Task<IEnumerable<StandardVersion>> GetStandardVersionsByIFateReferenceNumber(string iFateReferenceNumber);
        Task<IEnumerable<StandardVersion>> GetStandardVersionsByLarsCode(int larsCode);
        
        /// <summary>
        /// Method can take LarsCode, IFateReferenceNumber or StandardUId and will return a standard.
        /// If LarsCode or IFateReferenceNumber is supplied, One Standard, the latest version will 
        /// be returned, based on highest version number.
        /// </summary>
        /// <param name="id">LarsCode, IFateReferenceNumber or StandardUId</param>
        /// <returns></returns>
        Task<StandardVersion> GetStandardVersionById(string id);
        Task<IEnumerable<StandardVersion>> GetEpaoRegisteredStandardVersions(string epaOrgId);
        Task<IEnumerable<StandardVersion>> GetEpaoRegisteredStandardVersions(string epaOrgId, int larsCode);
        Task<IEnumerable<StandardVersion>> GetEpaoRegisteredStandardVersions(string epaOrgId, string iFateReferenceNumber);
        Task<StandardOptions> GetStandardOptions(string standardId);
    }
}