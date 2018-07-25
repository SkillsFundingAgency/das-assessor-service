using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities.ao;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Data
{
    public class RegisterRepository : IRegisterRepository
    {
        private readonly IWebConfiguration _configuration;

        public RegisterRepository(IWebConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task<IEnumerable<EpaOrganisationType>> GetOrganisationTypes()
        {
            var connectionString = _configuration.SqlConnectionString;
            
            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var orgTypes = connection.QueryAsync<EpaOrganisationType>("select * from [OrganisationType]").Result;
                return orgTypes;
            }
        }
    }
}
