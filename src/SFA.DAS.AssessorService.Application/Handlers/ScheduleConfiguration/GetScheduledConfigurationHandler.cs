using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.ScheduleConfiguration
{
    public class GetScheduledConfigurationHandler : IRequestHandler<GetScheduleConfigurationRequest, ScheduleConfigurationResponse>
    {
        private readonly IScheduleConfigurationQueryRepository _scheduleConfigurationQueryRepository;

        public GetScheduledConfigurationHandler(IScheduleConfigurationQueryRepository scheduleConfigurationQueryRepository)
        {
            _scheduleConfigurationQueryRepository = scheduleConfigurationQueryRepository;
        }

        public async Task<ScheduleConfigurationResponse> Handle(GetScheduleConfigurationRequest request, CancellationToken cancellationToken)
        {
            var scheduleConfigurationEntity = await _scheduleConfigurationQueryRepository.GetScheduleConfiguration();
            var scheduleConfiguration = Mapper.Map<ScheduleConfigurationResponse>(scheduleConfigurationEntity);
            return scheduleConfiguration;
        }
    }
}