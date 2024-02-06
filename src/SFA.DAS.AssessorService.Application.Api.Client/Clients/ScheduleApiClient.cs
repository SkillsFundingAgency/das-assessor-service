using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Domain.Paging;
using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.ScheduleRun;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class ScheduleApiClient : ApiClientBase, IScheduleApiClient
    {
        public ScheduleApiClient(HttpClient httpClient, IAssessorTokenService tokenService, ILogger<ApiClientBase> logger)
            : base(httpClient, tokenService, logger)
        {
        }

        public async Task<Domain.Entities.ScheduleRun> GetNextScheduledRun(int scheduleType)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"api/v1/schedule/next?scheduleType={scheduleType}"))
            {
                return await RequestAndDeserialiseAsync<Domain.Entities.ScheduleRun>(httpRequest, $"Could not retrieve schedule run for {scheduleType}");
            }
        }

        public async Task<object> RunNowScheduledRun(int scheduleType)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"api/v1/schedule/runnow?scheduleType={scheduleType}"))
            {
                return await PostPutRequestWithResponseAsync<object, object>(request, default(object));
            }
        }

        public async Task<object> CreateScheduleRun(Domain.Entities.ScheduleRun schedule)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, $"api/v1/schedule/create"))
            {
                return await PostPutRequestWithResponseAsync<Domain.Entities.ScheduleRun, object>(request, schedule);
            }
        }

        public async Task<Domain.Entities.ScheduleRun> GetScheduleRun(Guid scheduleRunId)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"api/v1/schedule?scheduleRunId={scheduleRunId}"))
            {
                return await RequestAndDeserialiseAsync<Domain.Entities.ScheduleRun>(httpRequest, $"Could not retrieve schedule run {scheduleRunId}");
            }
        }

        public async Task<IList<Domain.Entities.ScheduleRun>> GetAllScheduledRun(int scheduleType)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"api/v1/schedule/all?scheduleType={scheduleType}"))
            {
                return await RequestAndDeserialiseAsync<IList<Domain.Entities.ScheduleRun>>(httpRequest, $"Could not retrieve schedule runs for {scheduleType}");
            }
        }

        public async Task<object> DeleteScheduleRun(Guid scheduleRunId)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"api/v1/schedule?scheduleRunId={scheduleRunId}"))
            {
                return await DeleteAsync<object>(httpRequest);
            }
        }

        public async Task RestartSchedule(Guid id)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/schedule/updatelaststatus"))
            {
                var model = new UpdateLastRunStatusRequest { ScheduleRunId = id, LastRunStatus = LastRunStatus.Restarting };
                await PostPutRequestWithResponseAsync<object, object>(request, model);
            }
        }
    }
}
