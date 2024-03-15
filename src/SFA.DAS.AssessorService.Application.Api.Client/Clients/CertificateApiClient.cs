using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class CertificateApiClient : ApiClientBase, ICertificateApiClient
    {
        public CertificateApiClient(IAssessorApiClientFactory clientFactory, ILogger<CertificateApiClient> logger)
            : base(clientFactory.CreateHttpClient(), logger)
        {
        }

        public async Task<Certificate> Start(StartCertificateRequest request)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/v1/certificates/start"))
            {
                return await PostPutRequestWithResponseAsync<StartCertificateRequest, Certificate>(httpRequest, request);
            }
        }

        public async Task<Certificate> GetCertificate(Guid certificateId, bool includeLogs)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"api/v1/certificates/{certificateId}?includeLogs={includeLogs}"))
            {
                return await RequestAndDeserialiseAsync<Certificate>(httpRequest, "Could not get Certificate");
            }
        }

        public async Task<Certificate> UpdateCertificate(UpdateCertificateRequest certificateRequest)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Put, "api/v1/certificates/update"))
            {
                return await PostPutRequestWithResponseAsync<UpdateCertificateRequest, Certificate>(httpRequest, certificateRequest);
            }
        }

        public async Task<CertificateAddress> GetContactPreviousAddress(string epaOrgId, string employerAccountId)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"api/v1/certificates/contact/previousaddress?epaOrgId={epaOrgId}&employerAccountId={employerAccountId}"))
            {
                return await RequestAndDeserialiseAsync<CertificateAddress>(httpRequest, "Could not get Certificate Address");
            }
        }

        public async Task<PaginatedList<CertificateSummaryResponse>> GetCertificateHistory(int pageIndex, string endPointAssessorOrganisationId, string searchTerm, string sortColumn, int sortDescending)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"api/v1/certificates/history/?" +
                                                                            $"pageIndex={pageIndex}" +
                                                                            $"&searchTerm={searchTerm}" +
                                                                            $"&sortColumn={sortColumn}" +
                                                                            $"&sortDescending={sortDescending}" +
                                                                            $"&endPointAssessorOrganisationId={endPointAssessorOrganisationId}"))
            {
                return await RequestAndDeserialiseAsync<PaginatedList<CertificateSummaryResponse>>(httpRequest, "Could not get Certificate History");
            }
        }

        public async Task Delete(DeleteCertificateRequest deleteCertificateRequest)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Delete, "api/v1/certificates/DeleteCertificate"))
            {
                await PostPutRequestAsync(httpRequest, deleteCertificateRequest);
            }
        }

        public async Task<Certificate> UpdateCertificateRequestReprint(UpdateCertificateRequestReprintCommand command)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/v1/certificates/request-reprint"))
            {
                return await PostPutRequestWithResponseAsync<UpdateCertificateRequestReprintCommand, Certificate>(httpRequest, command);
            }
        }

        public async Task UpdateCertificateWithAmendReason(UpdateCertificateWithAmendReasonCommand command)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/v1/certificates/update-with-amend-reason"))
            {
                await PostPutRequestAsync(httpRequest, command);
            }
        }

        public async Task UpdateCertificateWithReprintReason(UpdateCertificateWithReprintReasonCommand command)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/v1/certificates/update-with-reprint-reason"))
            {
                await PostPutRequestAsync(httpRequest, command);
            }
        }
    }
}