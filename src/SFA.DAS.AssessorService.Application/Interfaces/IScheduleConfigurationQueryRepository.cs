using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IScheduleConfigurationQueryRepository
    {
        Task<ScheduleConfiguration> GetScheduleConfiguration();
    }
}