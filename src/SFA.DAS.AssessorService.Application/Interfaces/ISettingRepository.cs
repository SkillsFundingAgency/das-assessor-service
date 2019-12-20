using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface ISettingRepository
    {
        Task<string> GetSetting(string name);
        Task SetSetting(string name, string value);
    }
}
