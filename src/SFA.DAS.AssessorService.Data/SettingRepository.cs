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
                param: new
                {
                    Name = name
                },
                transaction: _unitOfWork.Transaction);

            return value;
        }

        public async Task<bool> SetSetting(string name, string value)
        {
            const string sql = "SELECT [Name] FROM [Settings] WHERE [Name] = @Name";
            var existingName = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<string>(
                sql,
                param: new
                {
                    Name = name
                },
                transaction: _unitOfWork.Transaction);

            if (existingName == null)
            {
                await _unitOfWork.Connection.ExecuteAsync(
                    "INSERT INTO [Settings] ([Value],[Name]) " +
                    $@"VALUES (@value, @name)",
                    param: new { value, name },
                    transaction: _unitOfWork.Transaction);
                
                return true;
            }
         
            await _unitOfWork.Connection.ExecuteAsync(
                "UPDATE [Settings] SET [Value] = @value " +
                "WHERE [Name] = @name",
                param: new { value, name },
                transaction: _unitOfWork.Transaction);
            
            return false;
        }
    }
}
