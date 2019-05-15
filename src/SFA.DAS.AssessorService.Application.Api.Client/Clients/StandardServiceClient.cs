﻿using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.Api.Types;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class StandardServiceClient : ApiClientBase, IStandardServiceClient
    {
        public StandardServiceClient(string baseUri, ITokenService tokenService, ILogger<StandardServiceClient> logger) : base(baseUri, tokenService, logger)
        {
        }

        public StandardServiceClient(HttpClient httpClient, ITokenService tokenService, ILogger<ApiClientBase> logger) : base(httpClient, tokenService, logger)
        {
        }

        public async Task<IEnumerable<StandardCollation>> GetAllStandards()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/standard-service/standards"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<StandardCollation>>(request,
                    $"Could not get the list of standards");
            }
        }

        public async Task<StandardCollation> GetStandard(int standardCode)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/standard-service/standards/{standardCode}"))
            {
                return await RequestAndDeserialiseAsync<StandardCollation>(request, $"Could not find the standard {standardCode}");
            }
        }
    }

    public interface IStandardServiceClient
    {
        Task<IEnumerable<StandardCollation>> GetAllStandards();
        Task<StandardCollation> GetStandard(int standardCode);
    }
}