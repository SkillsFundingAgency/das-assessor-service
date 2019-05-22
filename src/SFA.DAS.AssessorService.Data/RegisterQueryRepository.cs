﻿using SFA.DAS.AssessorService.Application.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using System;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;

namespace SFA.DAS.AssessorService.Data
{
    public class RegisterQueryRepository : IRegisterQueryRepository
    {
        private readonly IWebConfiguration _configuration;

        public RegisterQueryRepository(IWebConfiguration configuration)
        {
            _configuration = configuration;
            SqlMapper.AddTypeHandler(typeof(OrganisationData), new OrganisationDataHandler());
            SqlMapper.AddTypeHandler(typeof(OrganisationStandardData), new OrganisationStandardDataHandler());
        }

        public async Task<IEnumerable<OrganisationType>> GetOrganisationTypes()
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var orgTypes = await connection.QueryAsync<OrganisationType>("select * from [OrganisationType] where Status <> 'Deleted'  order by id");
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

                var deliveryAreas = await connection.QueryAsync<DeliveryArea>("select * from [DeliveryArea] order by ordering");
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
                    "SELECT O.Id, O.CreatedAt, O.DeletedAt, O.EndPointAssessorName as Name, O.EndPointAssessorOrganisationId as OrganisationId, O.EndPointAssessorUkprn as ukprn, " +
                    "O.PrimaryContact, C.DisplayName as PrimaryContactName, O.Status, O.UpdatedAt, O.OrganisationTypeId, O.OrganisationData, O.ApiEnabled, O.ApiUser " +
                    " FROM [Organisations] O " +
                    "LEFT OUTER JOIN [Contacts] C ON C.Username = O.PrimaryContact AND C.EndPointAssessorOrganisationId = O.EndPointAssessorOrganisationId " +
                    "WHERE O.Id = @id";
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
                    "SELECT O.Id, O.CreatedAt, O.DeletedAt, O.EndPointAssessorName as Name, O.EndPointAssessorOrganisationId as OrganisationId, O.EndPointAssessorUkprn as ukprn, " +
                    "O.PrimaryContact, C.DisplayName as PrimaryContactName, O.Status, O.UpdatedAt, O.OrganisationTypeId, O.OrganisationData, O.ApiEnabled, O.ApiUser " +
                    " FROM [Organisations] O " +
                    "LEFT OUTER JOIN [Contacts] C ON C.Username = O.PrimaryContact AND C.EndPointAssessorOrganisationId = O.EndPointAssessorOrganisationId " +
                    "WHERE O.EndPointAssessorOrganisationId = @organisationId";
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
                    "C.DisplayName, C.email, C.Status, C.UpdatedAt, C.Username, C.PhoneNumber, C.GivenNames as FirstName, C.FamilyName as LastName, " +
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
                    "SELECT O.Id, O.CreatedAt, O.DeletedAt, O.EndPointAssessorName as Name,  O.EndPointAssessorOrganisationId as OrganisationId, O.EndPointAssessorUkprn as ukprn, " +
                    "O.PrimaryContact, C.DisplayName as PrimaryContactName, O.Status, O.UpdatedAt, O.OrganisationTypeId, O.OrganisationData, O.ApiEnabled, O.ApiUser " +
                    " FROM [Organisations] O " +
                    "JOIN OrganisationStandard  OS ON OS.EndPointAssessorOrganisationId = O.EndPointAssessorOrganisationId " +
                    "LEFT OUTER JOIN [Contacts] C ON C.Username = O.PrimaryContact AND C.EndPointAssessorOrganisationId = O.EndPointAssessorOrganisationId " +
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
                    @"SELECT distinct os.*, sc.*
                FROM [OrganisationStandard] os
                    INNER JOIN StandardCollation sc ON sc.StandardId = os.StandardCode 
                WHERE EndPointAssessorOrganisationId = @organisationId";
                
                var standard = await connection.QueryAsync<OrganisationStandardSummary, StandardCollation, OrganisationStandardSummary>(
                    sqlForStandardByOrganisationId, (summary, collation) =>
                    {
                        summary.StandardCollation = new StandardCollation()
                        {
                            Title = collation.Title
                        };
                        return summary;
                    }, new {organisationId});
                
                return standard;
            }
        }

        public async Task<OrganisationStandard> GetOrganisationStandardFromOrganisationStandardId(int organisationStandardId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sqlForStandardByOrganisationStandardId =
                    "SELECT Id, EndPointAssessorOrganisationId as OrganisationId, StandardCode as StandardId, EffectiveFrom, EffectiveTo, " +
                    "DateStandardApprovedOnRegister, Comments, Status, ContactId, OrganisationStandardData "+
                    "FROM [OrganisationStandard] WHERE Id = @organisationStandardId";
                return await connection.QuerySingleAsync<OrganisationStandard>(sqlForStandardByOrganisationStandardId, new {organisationStandardId});
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
                      "SELECT o.EndPointAssessorOrganisationId as Id, o.EndPointAssessorName as Name, o.EndPointAssessorUkprn as ukprn, o.OrganisationData, ot.Id as OrganisationTypeId, ot.Type as OrganisationType, c.Email as Email "
                    + "FROM [Organisations] o "
                    + "LEFT OUTER JOIN [OrganisationType] ot ON ot.Id = o.OrganisationTypeId "
                    + "LEFT OUTER JOIN [Contacts] c ON c.Username = o.PrimaryContact AND c.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId "
                    + "WHERE o.EndPointAssessorUkprn = @ukprnNumeric";

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

                var sql =
                      "SELECT o.EndPointAssessorOrganisationId as Id, o.EndPointAssessorName as Name, o.EndPointAssessorUkprn as ukprn, o.OrganisationData, ot.Id as OrganisationTypeId, ot.Type as OrganisationType, c.Email as Email "
                    + "FROM [Organisations] o "
                    + "LEFT OUTER JOIN [OrganisationType] ot ON ot.Id = o.OrganisationTypeId "
                    + "LEFT OUTER JOIN [Contacts] c ON c.Username = o.PrimaryContact AND c.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId "
                    + "WHERE o.EndPointAssessorOrganisationId like @organisationId";

                var assessmentOrganisationSummaries = await connection.QueryAsync<AssessmentOrganisationSummary>(sql, new {organisationId = $"{organisationId.Replace(" ","")}" });
                return assessmentOrganisationSummaries;
            }
        }

        public async Task<AssessmentOrganisationSummary> GetAssessmentOrganisationByContactEmail(string email)
        {
            var connectionString = _configuration.SqlConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql =
                    "SELECT top 1 o.EndPointAssessorOrganisationId as Id, o.EndPointAssessorName as Name, o.EndPointAssessorUkprn as ukprn, o.OrganisationData, ot.Id as OrganisationTypeId, ot.Type as OrganisationType, pc.Email as Email "
                    + "FROM [Organisations] o "
                    + "LEFT OUTER JOIN [OrganisationType] ot ON ot.Id = o.OrganisationTypeId "
                    + "LEFT OUTER JOIN [Contacts] pc ON pc.Username = o.PrimaryContact AND pc.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId "
                    + "LEFT JOIN [Contacts] c ON c.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId "
                    + "WHERE replace(c.Email, ' ','')  = replace(@email, ' ','')";

                var organisation = await connection.QuerySingleOrDefaultAsync<AssessmentOrganisationSummary>(sql, new { email});
                return organisation;
            }
        }
        public async Task<IEnumerable<AssessmentOrganisationSummary>> GetAssessmentOrganisationsByNameOrCharityNumberOrCompanyNumber(string searchString)
        {
            var connectionString = _configuration.SqlConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql =
                      "SELECT o.EndPointAssessorOrganisationId as Id, o.EndPointAssessorName as Name, o.EndPointAssessorUkprn as ukprn, o.OrganisationData, ot.Id as OrganisationTypeId, ot.Type as OrganisationType, c.Email as Email "
                    + "FROM [Organisations] o "
                    + "LEFT OUTER JOIN [OrganisationType] ot ON ot.Id = o.OrganisationTypeId "
                    + "LEFT OUTER JOIN [Contacts] c ON c.Username = o.PrimaryContact AND c.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId "
                    + "WHERE replace(o.EndPointAssessorName, ' ','') like @searchString "
                    + "OR replace(JSON_VALUE(o.[OrganisationData], '$.TradingName'), ' ','') like @searchString "
                    + "OR replace(JSON_VALUE(o.[OrganisationData], '$.LegalName'), ' ','') like @searchString "
                    + "OR replace(JSON_VALUE(o.[OrganisationData], '$.CompanyNumber'), ' ','') like @searchString "
                    + "OR replace(JSON_VALUE(o.[OrganisationData], '$.CharityNumber'), ' ','') like @searchString ";
                var assessmentOrganisationSummaries = await connection.QueryAsync<AssessmentOrganisationSummary>(sql, new {searchString =$"%{searchString.Replace(" ","")}%" } );
                return assessmentOrganisationSummaries;
            }
        }

        public async Task<IEnumerable<int>> GetDeliveryAreaIdsByOrganisationStandardId(int organisationStandardId)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql =
                    "select DeliveryAreaId from organisationStandardDeliveryArea" +
                    " where OrganisationStandardId = @organisationStandardId";
                var deliveryAreas = await connection.QueryAsync<int>(sql, new {organisationStandardId});
                return deliveryAreas;
            }
        }

        public async Task<EpaContact> GetContactByContactId(Guid contactId)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql =
                    "select Id, EndPointAssessorOrganisationId, Username,GivenNames, DisplayName, FamilyName, SigninId, SigninType, Email, Status, PhoneNumber " +
                    " from Contacts where Id = @contactId";
                var contact = await connection.QuerySingleOrDefaultAsync<EpaContact>(sql, new { contactId });
                return contact;
            }
        }

        public async Task<EpaContact> GetContactByEmail(string email)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql =
                    "select Id, EndPointAssessorOrganisationId, Username,GivenNames, DisplayName, FamilyName, SigninId, SigninType, Email, Status, PhoneNumber " +
                    " from Contacts where Email = @email";
                var contact = await connection.QuerySingleOrDefaultAsync<EpaContact>(sql, new { email });
                return contact;
            }
        }

        public async Task<EpaContact> GetContactBySignInId(string signinId)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql =
                    "select Id, EndPointAssessorOrganisationId, Username,GivenNames, DisplayName, FamilyName, SigninId, SigninType, Email, Status, PhoneNumber " +
                    " from Contacts where SigninId = @signinId";
                var contact = await connection.QuerySingleOrDefaultAsync<EpaContact>(sql, new { signinId });
                return contact;
            }
        }

        public async Task<IEnumerable<OrganisationStandardDeliveryArea>> GetDeliveryAreasByOrganisationStandardId(
            int organisationStandardId)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql =
                    "select *  from organisationStandardDeliveryArea" +
                    " where OrganisationStandardId = @organisationStandardId";
                var deliveryAreas =
                    await connection.QueryAsync<OrganisationStandardDeliveryArea>(sql, new {organisationStandardId});
                return deliveryAreas;
            }
        }
    }
}