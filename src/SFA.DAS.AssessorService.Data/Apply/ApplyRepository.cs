using Dapper;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using SFA.DAS.AssessorService.Settings;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.Apply
{
    public class ApplyRepository : IApplyRepository
    {
        private readonly IWebConfiguration _configuration;
        private readonly ILogger<ApplyRepository> _logger;

        public ApplyRepository(IWebConfiguration configuration, ILogger<ApplyRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;

            SqlMapper.AddTypeHandler(typeof(ApplicationData), new ApplicationDataHandler());
        }

        public async Task<List<Domain.Entities.Application>> GetUserApplications(Guid userId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                return (await connection.QueryAsync<Domain.Entities.Application>(@"SELECT a.* FROM Contacts c
                                                    INNER JOIN Applications a ON a.OrganisationId = c.OrganisationId
                                                    WHERE c.Id = @userId AND a.CreatedBy = @userId", new { userId })).ToList();
            }
        }

        public async Task<List<Domain.Entities.Application>> GetOrganisationApplications(Guid userId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                return (await connection.QueryAsync<Domain.Entities.Application>(@"SELECT a.* FROM Contacts c
                                                    INNER JOIN Applications a ON a.OrganisationId = c.OrganisationId
                                                    WHERE c.Id = @userId", new { userId })).ToList();
            }
        }
    }
}
