using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Data
{
    public class RegisterValidationRepository: IRegisterValidationRepository
    {
        private readonly IWebConfiguration _configuration;

        public RegisterValidationRepository(IWebConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> EpaOrganisationExistsWithOrganisationId(string organisationId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                var sqlToCheckExists =
                    "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Organisations] " +
                    "WHERE EndPointAssessorOrganisationId = @organisationId";
                return await connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new {organisationId});
            }
        }

        public async Task<bool> EpaOrganisationExistsWithCompanyNumber(string organisationIdToExclude, string companyNumber)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                var sqlToCheckExists =
                    "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Organisations] " +
                    "WHERE JSON_VALUE(OrganisationData, '$.CompanyNumber') = @companyNumber " +
                    "AND EndPointAssessorOrganisationId != @organisationIdToExclude";
                return await connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { organisationIdToExclude, companyNumber });
            }
        }

        public async Task<bool> EpaOrganisationExistsWithCompanyNumber(string companyNumber)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                var sqlToCheckExists =
                    "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Organisations] " +
                    "WHERE JSON_VALUE(OrganisationData, '$.CompanyNumber') = @companyNumber";
                return await connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { companyNumber });
            }
        }

        public async Task<bool> EpaOrganisationExistsWithCharityNumber(string organisationIdToExclude, string charityNumber)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                var sqlToCheckExists =
                    "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Organisations] " +
                    "WHERE JSON_VALUE(OrganisationData, '$.CharityNumber') = @charityNumber " +
                    "AND EndPointAssessorOrganisationId != @organisationIdToExclude";
                return await connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { organisationIdToExclude, charityNumber });
            }
        }

        public async Task<bool> EpaOrganisationExistsWithCharityNumber(string charityNumber)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                var sqlToCheckExists =
                    "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Organisations] " +
                    "WHERE JSON_VALUE(OrganisationData, '$.CharityNumber') = @charityNumber";
                return await connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { charityNumber });
            }
        }

        public async Task<bool> EpaOrganisationExistsWithUkprn(long ukprn)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                var sqlToCheckExists =
                    "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Organisations] " +
                    "WHERE EndPointAssessorUkprn = @ukprn";
                return await connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new {ukprn});
            }
        }

        public async Task<bool> OrganisationTypeExists(int organisationTypeId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                var sqlToCheckExists =
                    "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [OrganisationType] " +
                    "WHERE Id = @organisationTypeId";
                return await connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new {organisationTypeId});
            }
        }
        
        public async Task<bool> EpaOrganisationAlreadyUsingUkprn(long ukprn, string organisationIdToExclude)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                var sqlToCheckExists =
                    "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Organisations] " +
                    "WHERE EndPointAssessorOrganisationId != @organisationIdToExclude and EndPointAssessorUkprn = @ukprn";
                return await connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { organisationIdToExclude, ukprn});
            }
        }
              
        public async Task<bool> EpaOrganisationStandardExists(string organisationId, int standardCode)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                var sqlToCheckExists =
                    "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [OrganisationStandard] " +
                    "WHERE EndPointAssessorOrganisationId = @organisationId and standardCode = @standardCode";
                return await connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new {organisationId, standardCode});
            }
        }

        public async Task<bool> EpaOrganisationAlreadyUsingName(string organisationName, string organisationIdToExclude)
        {
         
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                
                var sqlToCheckExists =
                    "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Organisations] " +
                    "WHERE EndPointAssessorName = @organisationName";
                
                if (!string.IsNullOrEmpty(organisationIdToExclude))
                {
                    sqlToCheckExists =  "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Organisations] " +
                                        "WHERE EndPointAssessorName = @organisationName AND  EndPointAssessorOrganisationId != @organisationIdToExclude";
                    return await connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { organisationName, organisationIdToExclude });
                }

                return await connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new {organisationName});
            }
        }

        public async Task<bool> ContactIdIsValid(Guid contactId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                var sqlToCheckExists =
                    "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Contacts] " +
                    "WHERE id  = @contactId";
                return await connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new {contactId});
            }
        }

        public async Task<bool> ContactIdIsValidForOrganisationId(Guid contactId, string organisationId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                var sqlToCheckExists =
                    "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Contacts] " +
                    "WHERE id = @ContactId and EndPointAssessorOrganisationId = @organisationId";
                return await connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new {contactId, organisationId});
            }
        }
        
        
        public async Task<bool> EmailAlreadyPresentInAnotherOrganisation(string email, string organisationId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                const string sqlToCheckExists =
                    "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Contacts] " +
                    "WHERE email  = @email and EndPointAssessorOrganisationId != @organisationId";
                return await connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new {email, organisationId});
            }
        }

        public async Task<bool> EmailAlreadyPresentInAnOrganisationNotAssociatedWithContact(string email, Guid contactId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                const string sqlToCheckExists =
                    "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Contacts] " +
                    "WHERE email  = @email and EndPointAssessorOrganisationId != (select EndPointAssessorOrganisationId from contacts where id=@contactId)";
                return await connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { email, contactId });
            }
        }

        public async Task<bool> ContactExists(Guid contactId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                const string sqlToCheckExists =
                    "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Contacts] " +
                    "WHERE id  = @contactId";
                return await connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { contactId});
            }
        }

        public async Task<bool> ContactDetailsAlreadyExist(string firstName, string lastName, string email, string phone, Guid? contactId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
               var sqlToCheckExists =
                    "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Contacts] " +
                    "WHERE GivenNames = @firstName and FamilyName = @lastName and email = @email";

                sqlToCheckExists = !string.IsNullOrEmpty(phone)
                    ? sqlToCheckExists + " and phonenumber = @phone"
                    : sqlToCheckExists + " and phonenumber is null";

                if (contactId != null)
                    sqlToCheckExists = sqlToCheckExists + " and id != @contactId ";

                return await connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { contactId, firstName, lastName, email, phone });
            }
        }

        public async Task<bool> EmailAlreadyPresent(string email)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                const string sqlToCheckExists =
                    "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Contacts] " +
                    "WHERE email  = @email";
                return await connection.ExecuteScalarAsync<bool>(sqlToCheckExists, new { email });
            }
        }
        
    }
}