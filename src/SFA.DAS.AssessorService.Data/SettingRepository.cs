using Dapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data
{
    public class SettingRepository : Repository, ISettingRepository
    {
        public SettingRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public async Task<string> GetSetting(string name)
        {
            const string sql = "SELECT [Value] FROM [Settings] WHERE [Name] = @Name";
            var value = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<string>(
                sql,
                param: new { name },
                transaction: _unitOfWork.Transaction);

            return value;
        }

        public async Task CreateSetting(string name, string value)
        {
            const string sql = "INSERT INTO [Settings] ([Value],[Name]) VALUES (@value, @name)";
            await _unitOfWork.Connection.ExecuteAsync(
                    sql,
                    param: new { value, name },
                    transaction: _unitOfWork.Transaction);
        }

        public async Task UpdateSetting(string name, string value)
        {
            const string sql = "UPDATE [Settings] SET [Value] = @value WHERE [Name] = @name";
            await _unitOfWork.Connection.ExecuteAsync(
                sql,
                param: new { value, name },
                transaction: _unitOfWork.Transaction);
        }
    }
}
