using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using SFA.DAS.AssessorService.ExternalApis.IFAStandards.Types;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Data
{
    public class StandardRepository : IStandardRepository
    {

        private readonly IWebConfiguration _configuration;

        public StandardRepository(IWebConfiguration configuration)
        {
            _configuration = configuration;
            SqlMapper.AddTypeHandler(typeof(StandardData), new StandardDataHandler());
        }

        public async Task<List<StandardCollation>> GetStandardCollations()
        {
                var connectionString = _configuration.SqlConnectionString;

                using (var connection = new SqlConnection(connectionString))
                {
                    if (connection.State != ConnectionState.Open)
                        await connection.OpenAsync();

                    var standards = await connection.QueryAsync<StandardCollation>("select * from [StandardCollation]");
                    return standards.ToList();
                }
            
        }

        public async Task<string> UpsertStandards(List<StandardCollation> standards)
        {

            var countInserted = 0;
            var countUpdated = 0;
            var countRemoved = 0;

            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();


                // Go get current standardCollation
                var currentStandards = await GetStandardCollations();

                var deletedStandards = new List<StandardCollation>();

                foreach (var standard in currentStandards)
                {
                    if (!standards.Any(s => s.StandardId == standard.StandardId))
                        deletedStandards.Add(standard);
                }

                foreach (var standard in deletedStandards)
                {
                    countRemoved++;
                    connection.Execute(
                        "Update [StandardCollation] set IsLive=0, DateRemoved=getutcdate() " +
                        "where StandardId = @standardId",
                        new { standard.StandardId}
                    );
                }

                foreach (var standard in standards)
                {
                    var isNew = true;
                    var standardData = JsonConvert.SerializeObject(standard.StandardData);
                    if (currentStandards.Any(x => x.StandardId == standard.StandardId))
                        isNew = false;


                    if (isNew)
                    { 
                        countInserted++;
                        connection.Execute(
                            "INSERT INTO [StandardCollation] ([StandardId],[ReferenceNumber] ,[Title],[StandardData]) " +
                            $@"VALUES (@standardId, @referenceNumber, @Title, @standardData)",
                            new {standard.StandardId, standard.ReferenceNumber, standard.Title, standardData}
                        );
                    }
                    else
                    {
                        countUpdated++;
                        connection.Execute(
                            "Update [StandardCollation] set ReferenceNumber = @referenceNumber, Title = @Title, StandardData = @StandardData, DateUpdated=getutcdate(), DateRemoved=null, IsLive = 1 " +
                            "where StandardId = @standardId",
                            new { standard.StandardId, standard.ReferenceNumber, standard.Title, standardData }
                        );
                    }
                }

            }

            return $"details of update: Number of Inserts: {countInserted}; Number of Updates: {countUpdated}; Number of Removes: {countRemoved}";
        }
    }

    public enum InsertUpdateOrDelete
    {
        Insert,
        Update
    }
}
