using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class CertificateApiClient : ApiClientBase, ICertificateApiClient
    {
        public CertificateApiClient(string baseUri, ITokenService tokenService, ILogger<ApiClientBase> logger) : base(baseUri, tokenService, logger)
        {
        }

        public async Task<Certificate> Start(StartCertificateRequest request)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/v1/certificate"))
            {
                return await PostPutRequestWithResponse<StartCertificateRequest, Certificate>(httpRequest, request);
            }
        }

        public async Task<Certificate> GetCertificate(Guid certificateId)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"api/v1/certificate/{certificateId}"))
            {
                return await RequestAndDeserialiseAsync<Certificate>(httpRequest, "Could not get Certificate");
            }
        }

        public async Task<Certificate> UpdateCertificate(Certificate certificate)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Put, "api/v1/certificate"))
            {
                return await PostPutRequestWithResponse<Certificate, Certificate>(httpRequest,certificate);
            }
        }
    }
}