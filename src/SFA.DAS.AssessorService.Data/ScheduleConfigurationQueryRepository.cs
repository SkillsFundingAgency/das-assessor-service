using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data
{
    public class ScheduleConfigurationQueryRepository : IScheduleConfigurationQueryRepository
    {
        private readonly AssessorDbContext _assessorDbContext;       

        public ScheduleConfigurationQueryRepository(AssessorDbContext assessorDbContext)
        {
            _assessorDbContext = assessorDbContext;
        }

        public async Task<ScheduleConfiguration> GetScheduleConfiguration()
        {
            var scheduleConfiguration = await _assessorDbContext.ScheduleConfigurations.FirstAsync();
            return scheduleConfiguration;
        }
    }
}