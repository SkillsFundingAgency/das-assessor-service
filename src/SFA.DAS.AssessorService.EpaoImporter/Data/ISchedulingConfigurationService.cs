using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.EpaoImporter.Data
{
    public interface ISchedulingConfigurationService
    {
        Task<ScheduleConfiguration> GetSchedulingConfiguration();
    }
}