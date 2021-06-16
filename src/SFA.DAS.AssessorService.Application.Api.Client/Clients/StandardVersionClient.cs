using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class StandardVersionClient : ApiClientBase, IStandardVersionClient
    {
        public StandardVersionClient(string baseUri, ITokenService tokenService, ILogger<StandardServiceClient> logger) : base(baseUri, tokenService, logger)
        {
        }

        public StandardVersionClient(HttpClient httpClient, ITokenService tokenService, ILogger<ApiClientBase> logger) : base(httpClient, tokenService, logger)
        {
        }

        public async Task<IEnumerable<StandardVersion>> GetAllStandardVersions()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/standard-version/standards"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<StandardVersion>>(request, $"Could not get the list of standards");
            }
        }
        
        public async Task<IEnumerable<StandardVersion>> GetStandardVersionsByLarsCode(int larsCode)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/standard-version/standards/versions/{larsCode}"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<StandardVersion>>(request, $"Could not find the standard {larsCode}");
            }
        }

        public async Task<StandardVersion> GetStandardVersionById(string id)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/standard-version/standards/{id}"))
            {
                return await RequestAndDeserialiseAsync<StandardVersion>(request, $"Could not find the standard {id}");
            }
        }

        public async Task<IEnumerable<StandardVersion>> GetEpaoRegisteredStandardVersions(string epaOrgId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/standard-version/standards/epao/{epaOrgId}"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<StandardVersion>>(request, $"Could not find the registered standards versions for EPAO {epaOrgId}");
            }
        }

        public async Task<IEnumerable<StandardVersion>> GetEpaoRegisteredStandardVersions(string epaOrgId, int larsCode)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/standard-version/standards/epao/{epaOrgId}/{larsCode}"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<StandardVersion>>(request, $"Could not find the registered standards versions for EPAO {epaOrgId} and larsCode {larsCode}");
            }
        }

        public async Task<StandardOptions> GetStandardOptions(string standardId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/standard-version/standard-options/{standardId}"))
            {
                return await RequestAndDeserialiseAsync<StandardOptions>(request, $"Could not find the standard options for {standardId}");
            }
        }

    }

    public interface IStandardVersionClient
    {
        Task<IEnumerable<StandardVersion>> GetAllStandardVersions();
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
        Task<StandardOptions> GetStandardOptions(string standardId);
    }
}