using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class StaffReportsApiClient : ApiClientBase, IStaffReportsApiClient
    {
        public StaffReportsApiClient(IAssessorApiClientFactory clientFactory, ILogger<StaffReportsApiClient> logger) 
            : base(clientFactory.CreateHttpClient(), logger)
        {
        }

        public async Task<IEnumerable<StaffReport>> GetReportList()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/staffreports"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<StaffReport>>(request, $"Could not retrieve staff reports list");
            }
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetReport(Guid reportId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/staffreports/{reportId}"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<IDictionary<string, object>>>(request, $"Could not retrieve staff report {reportId}");
            }
        }

        public async Task<ReportType> GetReportTypeFromId(Guid reportId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/staffreports/{reportId}/report-type"))
            {
                return await RequestAndDeserialiseAsync<ReportType>(request, $"Could not retrieve staff report {reportId} report type");
            }
        }

        public async Task<ReportDetails> GetReportDetailsFromId(Guid reportId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/staffreports/{reportId}/report-details"))
            {
                return await RequestAndDeserialiseAsync<ReportDetails>(request, $"Could not retrieve staff report {reportId} report details");
            }
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetDataFromStoredProcedure(string storedProcedure)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/staffreports/report-content/{storedProcedure}"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<IDictionary<string, object>>>(request, $"Could not retrieve staff report for {storedProcedure}");
            }
        }
    }
}
