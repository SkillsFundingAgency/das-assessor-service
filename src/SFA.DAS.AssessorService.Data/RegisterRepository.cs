using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities.ao;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.AssessorService.Data
{
    public class RegisterRepository : IRegisterRepository
    {
        private readonly AssessorDbContext _assessorDbContext;

        public RegisterRepository(AssessorDbContext assessorDbContext)
        {
            _assessorDbContext = assessorDbContext;
        }

        public Task<List<EpaOrganisationType>> GetOrganisationTypes()
        {
            var connectionString = _assessorDbContext.Database.GetDbConnection().ConnectionString;

            using (var connection = new SqlConnection(connectionString))
                {
                    if (connection.State != ConnectionState.Open)
                        connection.Open();
                    //connection.Query()
                    var orgTypes = connection.Query<EpaOrganisationType>("select * from [ao].[OrganisationType]").ToList();

                    connection.Close();

                    return orgTypes;
                }
        }
    }
}
