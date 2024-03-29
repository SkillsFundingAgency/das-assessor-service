﻿using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class StandardVersionApiClient : ApiClientBase, IStandardVersionApiClient
    {
        public StandardVersionApiClient(IAssessorApiClientFactory clientFactory, ILogger<StandardVersionApiClient> logger) 
            : base(clientFactory.CreateHttpClient(), logger)
        {
        }

        public async Task<IEnumerable<StandardVersion>> GetAllStandardVersions()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/standard-version/standards"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<StandardVersion>>(request, $"Could not get the list of standards");
            }
        }

        public async Task<IEnumerable<StandardVersion>> GetLatestStandardVersions()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/standard-version/standards/latest"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<StandardVersion>>(request, $"Could not get the list of latest standards");
            }
        }

        public async Task<IEnumerable<StandardVersion>> GetStandardVersionsByIFateReferenceNumber(string iFateReferenceNumber)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/standard-version/standards/versions/{iFateReferenceNumber}"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<StandardVersion>>(request, $"Could not find the standard {iFateReferenceNumber}");
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

        public async Task<IEnumerable<StandardVersion>> GetEpaoRegisteredStandardVersions(string epaOrgId, string iFateReferenceNumber)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/standard-version/standards/epao/{epaOrgId}/{iFateReferenceNumber}"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<StandardVersion>>(request, $"Could not find the registered standards versions for EPAO {epaOrgId} and IFateReferenceNumber {iFateReferenceNumber}");
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
}