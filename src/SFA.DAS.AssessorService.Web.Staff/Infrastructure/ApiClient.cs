using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure
{
    public class ApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<ApiClient> _logger;

        public ApiClient(HttpClient client, ILogger<ApiClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<List<CertificateResponse>> GetCertificates()
        {
            var res = await _client.GetAsync(new Uri("/api/v1/certificates?statusses=Submitted", UriKind.Relative));
            return await res.Content.ReadAsAsync<List<CertificateResponse>>();
        }

        public async Task<PaginatedList<StaffSearchResult>> Search(string searchString, int page)
        {
            var res = await _client.GetAsync(new Uri($"/api/v1/staffsearch?searchQuery={searchString}&page={page}", UriKind.Relative));
            return await res.Content.ReadAsAsync<PaginatedList<StaffSearchResult>>();
        }

        public async Task<LearnerDetail> GetLearner(int stdCode, long uln)
        {
            var res = await _client.GetAsync(new Uri($"/api/v1/learnerDetails?stdCode={stdCode}&uln={uln}", UriKind.Relative));
            return await res.Content.ReadAsAsync<LearnerDetail>();
        }
    }
}