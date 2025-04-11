using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.Interfaces
{
    public interface ISettingRepository
    {
        Task<string> GetSetting(string name);
        Task CreateSetting(string name, string value);
        Task UpdateSetting(string name, string value);
    }
}
