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
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SFA.DAS.AssessorService.Data
{
    public class RegisterRepository : IRegisterRepository
    {
        private readonly AssessorDbContext _assessorDbContext;

        public RegisterRepository(AssessorDbContext assessorDbContext)
        {
            _assessorDbContext = assessorDbContext;
        }

        public async Task<IEnumerable<EpaOrganisationType>> GetOrganisationTypes()
        {
            var connectionString = _assessorDbContext.Database.GetDbConnection().ConnectionString;

           



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
