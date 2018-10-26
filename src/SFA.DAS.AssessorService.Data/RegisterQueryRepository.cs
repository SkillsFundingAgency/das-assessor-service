using SFA.DAS.AssessorService.Application.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using System;

namespace SFA.DAS.AssessorService.Data
{
    public class RegisterQueryRepository : IRegisterQueryRepository
    {
        private readonly IWebConfiguration _configuration;

        public RegisterQueryRepository(IWebConfiguration configuration)
        {
            _configuration = configuration;
            SqlMapper.AddTypeHandler(typeof(OrganisationData), new OrganisationDataHandler());
        }

        public async Task<IEnumerable<OrganisationType>> GetOrganisationTypes()
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var orgTypes = await connection.QueryAsync<OrganisationType>("select * from [OrganisationType] order by case Type when 'Other' then id + 1000 else id end");
                return orgTypes;
            }
        }

        public async Task<IEnumerable<DeliveryArea>> GetDeliveryAreas()
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var deliveryAreas = await connection.QueryAsync<DeliveryArea>("select * from [DeliveryArea]");
                return deliveryAreas;
            }
         }

        public async Task<EpaOrganisation> GetEpaOrganisationById(Guid id)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                var sqlForMainDetails =
                    "select Id, CreatedAt, DeletedAt, EndPointAssessorName as Name,  EndPointAssessorOrganisationId as OrganisationId, EndPointAssessorUkprn as ukprn, " +
                    "primaryContact, Status, UpdatedAt, OrganisationTypeId, OrganisationData " +
                    " FROM [Organisations] " +
                    "WHERE Id = @id";
                var orgs = await connection.QueryAsync<EpaOrganisation>(sqlForMainDetails, new {id});
                var org = orgs.FirstOrDefault();
                return org;
            }
        }

        public async Task<EpaOrganisation> GetEpaOrganisationByOrganisationId(string organisationId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                var sqlForMainDetails =
                    "select Id, CreatedAt, DeletedAt, EndPointAssessorName as Name,  EndPointAssessorOrganisationId as OrganisationId, EndPointAssessorUkprn as ukprn, " +
                    "primaryContact, Status, UpdatedAt, OrganisationTypeId, OrganisationData " +
                    " FROM [Organisations] " +
                    "WHERE EndPointAssessorOrganisationId = @organisationId";
                var orgs = await connection.QueryAsync<EpaOrganisation>(sqlForMainDetails, new {organisationId});
                var org = orgs.FirstOrDefault();
                return org;
            }
        }

        
        public async Task<string> EpaOrganisationIdCurrentMaximum()
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                const string sqlToGetHighestOrganisationId = "select max(EndPointAssessorOrganisationId) OrgId from organisations where EndPointAssessorOrganisationId like 'EPA%' " + 
                                                " and isnumeric(replace(EndPointAssessorOrganisationId,'EPA','')) = 1";
                return await connection.ExecuteScalarAsync<string>(sqlToGetHighestOrganisationId);
            }
        }

        public async Task<int> EpaContactUsernameHighestCounter()
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                const string sqlToGetHighestUsernameCounter = "select max(convert(int,replace(username,'unknown-',''))) highestCounter from [Contacts]  where username like 'unknown-%' and isnumeric(replace(username,'unknown-','')) = 1";
                var maxCounter = await connection.ExecuteScalarAsync<int?>(sqlToGetHighestUsernameCounter);

                return maxCounter ?? 100;
            }
        }

        public async Task<IEnumerable<AssessmentOrganisationSummary>> GetAssessmentOrganisations()
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var assessmentOrganisationSummaries =
                    await connection.QueryAsync<AssessmentOrganisationSummary>(
                        "select EndPointAssessorOrganisationId as Id, EndPointAssessorName as Name, EndPointAssessorUkprn as ukprn, OrganisationData from [Organisations]");
                return assessmentOrganisationSummaries;
            }
        }

        public async Task<IEnumerable<AssessmentOrganisationContact>> GetAssessmentOrganisationContacts(string organisationId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql =
                    "SELECT C.Id, C.EndPointAssessorOrganisationId as OrganisationId, C.CreatedAt, C.DeletedAt, " +
                    "C.DisplayName, C.email, C.Status, C.UpdatedAt, C.Username, C.PhoneNumber, " +
                    "CASE WHEN PrimaryContact Is NULL THEN 0 ELSE 1 END AS IsPrimaryContact " +
                    "from contacts C  left outer join Organisations O on " +
                    "C.Username = O.PrimaryContact AND C.EndPointAssessorOrganisationId = O.EndPointAssessorOrganisationId " +
                    "where C.EndPointAssessorOrganisationId = @organisationId " +
                    "order by CASE WHEN PrimaryContact Is NULL THEN 0 ELSE 1 END DESC";

                return await connection.QueryAsync<AssessmentOrganisationContact>(sql, new {organisationId});
            }
        }

        public async Task<AssessmentOrganisationContact> GetAssessmentOrganisationContact(Guid contactId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql =
                    "SELECT C.Id, C.EndPointAssessorOrganisationId as OrganisationId, C.CreatedAt, C.DeletedAt, " +
                    "C.DisplayName, C.email, C.Status, C.UpdatedAt, C.Username, C.PhoneNumber, " +
                    "CASE WHEN PrimaryContact Is NULL THEN 0 ELSE 1 END AS IsPrimaryContact " +
                    "from contacts C  left outer join Organisations O on " +
                    "C.Username = O.PrimaryContact AND C.EndPointAssessorOrganisationId = O.EndPointAssessorOrganisationId " +
                    "where convert(varchar(50),C.Id) = @contactId ";

                var contacts = await connection.QueryAsync<AssessmentOrganisationContact>(sql, new {contactId});
                return contacts.FirstOrDefault();
            }
        }

        public async Task<AssessmentOrganisationContact> GetPrimaryOrFirstContact(string organisationId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql =
                    "SELECT top 1 C.Id, C.EndPointAssessorOrganisationId as OrganisationId, C.CreatedAt, C.DeletedAt, " +
                    "C.DisplayName, C.email, C.Status, C.UpdatedAt, C.Username, C.PhoneNumber, " +
                    "CASE WHEN PrimaryContact Is NULL THEN 0 ELSE 1 END AS IsPrimaryContact " +
                    "from contacts C  left outer join Organisations O on " +
                    "C.Username = O.PrimaryContact AND C.EndPointAssessorOrganisationId = O.EndPointAssessorOrganisationId " +
                    "where C.EndPointAssessorOrganisationId = @organisationId " +
                    "order by CASE WHEN PrimaryContact Is NULL THEN 0 ELSE 1 END DESC";

                var contact = await connection.QuerySingleAsync<AssessmentOrganisationContact>(sql, new {organisationId});

                return contact;
            }
        }

     
        
        public async Task<IEnumerable<EpaOrganisation>> GetAssessmentOrganisationsByStandardId(int standardId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sqlForOrganisationsByStandardId =
                    "select O.Id, O.CreatedAt, O.DeletedAt, O.EndPointAssessorName as Name,  O.EndPointAssessorOrganisationId as OrganisationId, O.EndPointAssessorUkprn as ukprn, " +
                    "O.primaryContact, O.Status, O.UpdatedAt, O.OrganisationTypeId, O.OrganisationData " +
                    " FROM [Organisations] O " +
                    "JOIN OrganisationStandard  OS ON OS.EndPointAssessorOrganisationId = O.EndPointAssessorOrganisationId " +
                    "WHERE OS.StandardCode = @standardId";
                return await connection.QueryAsync<EpaOrganisation>(sqlForOrganisationsByStandardId, new {standardId});
            }
        }

        public async Task<IEnumerable<OrganisationStandardSummary>> GetOrganisationStandardByOrganisationId(string organisationId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sqlForStandardByOrganisationId =
                     "SELECT distinct EndPointAssessorOrganisationId as organisationId, StandardCode, EffectiveFrom, EffectiveTo, DateStandardApprovedOnRegister, ContactId, OrganisationData " +
                     "FROM [OrganisationStandard] WHERE EndPointAssessorOrganisationId = @organisationId";
                return await connection.QueryAsync<OrganisationStandardSummary>(sqlForStandardByOrganisationId, new {organisationId});
            }
        }

        public async Task<IEnumerable<OrganisationStandardPeriod>> GetOrganisationStandardPeriodsByOrganisationStandard(string organisationId, int standardId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql =
                    "SELECT EffectiveFrom, EffectiveTo " +
                    "FROM [OrganisationStandard] WHERE EndPointAssessorOrganisationId = @organisationId and StandardCode = @standardId";
                return await connection.QueryAsync<OrganisationStandardPeriod>(sql, new {organisationId, standardId});
            }
        }

        public async Task<IEnumerable<AssessmentOrganisationSummary>> GetAssessmentOrganisationsByUkprn(string ukprn)
        {
            var connectionString = _configuration.SqlConnectionString;
            if (!int.TryParse(ukprn.Replace(" ",""), out int ukprnNumeric))
            {
                return new List<AssessmentOrganisationSummary>();
            }
            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql =
                    "select EndPointAssessorOrganisationId as Id, EndPointAssessorName as Name, EndPointAssessorUkprn as ukprn, OrganisationData from [Organisations] where EndPointAssessorUkprn = @ukprnNumeric";

                var assessmentOrganisationSummaries = await connection.QueryAsync<AssessmentOrganisationSummary>(sql, new {ukprnNumeric});
                return assessmentOrganisationSummaries;
            }
        }
        public async Task<IEnumerable<AssessmentOrganisationSummary>> GetAssessmentOrganisationsByOrganisationId(string organisationId)
        {
            var connectionString = _configuration.SqlConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                var assessmentOrganisationSummaries = await connection.QueryAsync<AssessmentOrganisationSummary>("select EndPointAssessorOrganisationId as Id, EndPointAssessorName as Name, EndPointAssessorUkprn as ukprn, OrganisationData from [Organisations] where EndPointAssessorOrganisationId like @organisationId", new {organisationId = $"{organisationId.Replace(" ","")}%" });
                return assessmentOrganisationSummaries;
            }
        }
        public async Task<IEnumerable<AssessmentOrganisationSummary>> GetAssessmentOrganisationsbyName(string organisationName)
        {
            var connectionString = _configuration.SqlConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                var assessmentOrganisationSummaries = await connection.QueryAsync<AssessmentOrganisationSummary>("select EndPointAssessorOrganisationId as Id, EndPointAssessorName as Name, EndPointAssessorUkprn as ukprn, OrganisationData from [Organisations] where replace(EndPointAssessorName, ' ','') like @organisationName", new {organisationName =$"%{organisationName.Replace(" ","")}%" } );
                return assessmentOrganisationSummaries;
            }
        }

       
    }
}
