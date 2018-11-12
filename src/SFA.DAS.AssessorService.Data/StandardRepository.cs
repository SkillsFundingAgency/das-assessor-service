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

        public Task<IEnumerable<StandardCollation>> GetStandardCollations()
        {
            throw new System.NotImplementedException();
        }

        public async Task<string> UpsertStandards(List<StandardCollation> standards)
        {

            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var countInserted = 0;
                var countUpdated = 0;
                var countRemoved = 0;

                // Go get current standardCollation
                var currentStandards = new List<StandardCollation>();

                var deletedStandards = new List<StandardCollation>();

                foreach (var standard in currentStandards)
                {
                    if (!standards.Any(s => s.StandardId == standard.StandardId))
                        deletedStandards.Add(standard);
                }
                
                // delete the deletedStandards
                // MFCMFC

                foreach (var standard in standards)
                {
                    var standardData = JsonConvert.SerializeObject(standard.StandardData);
                    var upsertStatus = currentStandards.Any(x=> x.StandardId == standard.StandardId) 
                        ? InsertUpdateOrDelete.Update : 
                        InsertUpdateOrDelete.Insert;


                    if (upsertStatus == InsertUpdateOrDelete.Insert)
                        connection.Execute(
                            "INSERT INTO [StandardCollation] ([StandardId],[ReferenceNumber] ,[Title],[StandardData]) " +
                            $@"VALUES (@standardId, @referenceNumber, @Title, @standardData)",
                            new {standard.StandardId, standard.ReferenceNumber, standard.Title, standardData}
                        );
                }
                //return org.OrganisationId;

            }


            return "details of update";
        }
    }

    public enum InsertUpdateOrDelete
    {
        Insert,
        Update
    }
}
