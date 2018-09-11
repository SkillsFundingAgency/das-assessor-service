using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class CertificateApiClient : ApiClientBase, ICertificateApiClient
    {
        public CertificateApiClient(string baseUri, ITokenService tokenService, ILogger<ApiClientBase> logger) : base(baseUri, tokenService, logger)
        {
        }

        public async Task<Certificate> Start(StartCertificateRequest request)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/v1/certificates/start"))
            {
                return await PostPutRequestWithResponse<StartCertificateRequest, Certificate>(httpRequest, request);
            }
        }

        public async Task<Certificate> StartPrivate(StartCertificatePrivateRequest request)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/v1/certificates/startprivate"))
            {
                return await PostPutRequestWithResponse<StartCertificatePrivateRequest, Certificate>(httpRequest, request);
            }
        }

        public async Task<Certificate> GetCertificate(Guid certificateId)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"api/v1/certificates/{certificateId}"))
            {
                return await RequestAndDeserialiseAsync<Certificate>(httpRequest, "Could not get Certificate");
            }
        }

        public async Task<Certificate> UpdateCertificate(UpdateCertificateRequest certificateRequest)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Put, "api/v1/certificates/update"))
            {
                return await PostPutRequestWithResponse<UpdateCertificateRequest, Certificate>(httpRequest, certificateRequest);
                return await PostPutRequestWithResponse<UpdateCertificateRequest, Certificate>(httpRequest, certificateRequest);
            }
        }

        public async Task<CertificateAddress> GetContactPreviousAddress(string userName)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"api/v1/certificates/contact/previousaddress?username={userName}"))
            {
                return await RequestAndDeserialiseAsync<CertificateAddress>(httpRequest, "Could not get Certificate Address");
            }
        }

        public async Task<List<CertificateAddress>> GetPreviousAddressess(string userName)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"api/v1/certificates/addresses?userName={userName}"))
            {
                return await RequestAndDeserialiseAsync<List<CertificateAddress>>(httpRequest, "Could not get Certificate Addressess");
            }
        }

        public async Task<PaginatedList<CertificateHistoryResponse>> GetCertificateHistory(int pageIndex, string userName)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"api/v1/certificates/history/?pageIndex={pageIndex}&userName={userName}"))
            {
                return await RequestAndDeserialiseAsync<PaginatedList<CertificateHistoryResponse>>(httpRequest, "Could not get Certificate History");
            }
        }

        public async Task<List<Option>> GetOptions(int stdCode)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"api/v1/certificates/options/?stdCode={stdCode}"))
            {
                return await RequestAndDeserialiseAsync<List<Option>>(httpRequest, "Could not get Options");
            }
        }
    }
}