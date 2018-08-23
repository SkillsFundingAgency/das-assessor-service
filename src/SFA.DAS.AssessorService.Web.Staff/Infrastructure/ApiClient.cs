﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure
{
    public class ApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<ApiClient> _logger;
        private readonly ITokenService _tokenService;

        public ApiClient(HttpClient client, ILogger<ApiClient> logger, ITokenService tokenService)
        {
            _client = client;
            _logger = logger;
            _tokenService = tokenService;
        }

        private async Task<T> Get<T>(string uri)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var res = await _client.GetAsync(new Uri(uri, UriKind.Relative));
            return await res.Content.ReadAsAsync<T>();
        }

        protected async Task<U> Post<T, U>(string uri, T model)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            var serializeObject = JsonConvert.SerializeObject(model);
            
            using (var response = await _client.PostAsync(new Uri(uri, UriKind.Relative), new StringContent(serializeObject,System.Text.Encoding.UTF8, "application/json")))
            {
                return await response.Content.ReadAsAsync<U>();
            }
        }

        public async Task<List<CertificateResponse>> GetCertificates()
        {
            return await Get<List<CertificateResponse>>("/api/v1/certificates?statusses=Submitted");
        }

        public async Task<PaginatedList<StaffSearchResult>> Search(string searchString, int page)
        {
            return await Get<PaginatedList<StaffSearchResult>>($"/api/v1/staffsearch?searchQuery={searchString}&page={page}");
        }

        public async Task<PaginatedList<StaffBatchSearchResult>> BatchSearch(int batchNumber, int page)
        {
            return await Get<PaginatedList<StaffBatchSearchResult>>($"/api/v1/staffsearch/batch?batchNumber={batchNumber}&page={page}");
        }

        public async Task<PaginatedList<StaffBatchLogResult>> BatchLog(int page)
        {
            return await Get<PaginatedList<StaffBatchLogResult>>($"/api/v1/staffsearch/batchlog?page={page}");
        }

        public async Task<LearnerDetail> GetLearner(int stdCode, long uln, bool allLogs)
        {
            return await Get<LearnerDetail>($"/api/v1/learnerDetails?stdCode={stdCode}&uln={uln}&alllogs={allLogs}");
        }

        public async Task<Certificate> GetCertificate(Guid certificateId)
        {
            return await Get<Certificate>($"api/v1/certificates/{certificateId}");
        }

        public async Task<ScheduleRun> GetNextScheduleToRunNow()
        {
            return await Get<ScheduleRun>($"api/v1/schedule?scheduleType=1");
        }

        public async Task<ScheduleRun> GetNextScheduledRun()
        {
            return await Get<ScheduleRun>($"api/v1/schedule/next?scheduleType=1");
        }

        public async Task<Certificate> PostReprintRequest(StaffCertificateDuplicateRequest staffCertificateDuplicateRequest)
        {
            return await Post<StaffCertificateDuplicateRequest, Certificate>("api/v1/staffcertificatereprint", staffCertificateDuplicateRequest);   
        }
    }
}