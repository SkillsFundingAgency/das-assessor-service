using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using SFA.DAS.AssessorService.Application.Auditing;

namespace SFA.DAS.AssessorService.Data
{
    public class AuditRepository : IAuditRepository
    {
        private readonly IWebConfiguration _configuration;
        private readonly ILogger<AuditRepository> _logger;

        public AuditRepository(IWebConfiguration configuration, ILogger<AuditRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task CreateAudit(Guid organisationId, string updatedBy, List<AuditChange> auditChanges)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                foreach (var auditChange in auditChanges)
                {
                    var auditData = JsonConvert.SerializeObject(new AuditData
                    {
                        FieldChanged = auditChange.FieldChanged,
                        PreviousValue = auditChange.PreviousValue,
                        CurrentValue = auditChange.CurrentValue
                    });

                    connection.Execute(
                        "INSERT INTO [Audits] ([OrganisationId],[UpdatedBy],[UpdatedAt],[AuditData]) " +
                        "VALUES (@organisationId, @updatedBy, GetUtcDate(), @auditData)",
                        new { organisationId, updatedBy, auditData }
                    );
                }
            }
        }
    }
}
