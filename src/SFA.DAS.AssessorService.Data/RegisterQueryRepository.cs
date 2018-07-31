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

namespace SFA.DAS.AssessorService.Data
{
    public class RegisterQueryRepository : IRegisterQueryRepository
    {
        private readonly IWebConfiguration _configuration;

        public RegisterQueryRepository(IWebConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<OrganisationType>> GetOrganisationTypes()
        {
            var connectionString = _configuration.SqlConnectionString;
            
            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var orgTypes = await connection.QueryAsync<OrganisationType>("select * from [OrganisationType]");
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

        public async Task<IEnumerable<AssessmentOrganisationSummary>> GetAssessmentOrganisations()
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var assessmentOrganisationSummaries = await connection.QueryAsync<AssessmentOrganisationSummary>("select EndPointAssessorOrganisationId as Id, EndPointAssessorName as Name, EndPointAssessorUkprn as ukprn from [Organisations]");
                return assessmentOrganisationSummaries;
            }
        }

        public async Task<AssessmentOrganisationDetails> GetAssessmentOrganisation(string organisationId)
        {

            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sqlForMainDetails =
                    "select EndPointAssessorOrganisationId as Id, EndPointAssessorName as Name, EndPointAssessorUkprn as ukprn, " +
                    "O.OrganisationTypeId, OT.Type OrganisationType,  JSON_VALUE(OrganisationData, '$.WebsiteLink') AS Website, " +
                    "O.Status FROM [Organisations] O LEFT OUTER JOIN [OrganisationType] OT ON O.OrganisationTypeId = OT.Id " +
                    $@"WHERE EndPointAssessorOrganisationId = '{organisationId}'";
                var orgs = await connection.QueryAsync<AssessmentOrganisationDetails>(sqlForMainDetails);
                var org = orgs.FirstOrDefault();

                var addresses = await GetAssessmentOrganisationAddresses(organisationId);
                org.Address = addresses.FirstOrDefault();

                var contact = await GetPrimaryOrFirstContact(organisationId);

                if (contact == null) return org;

                org.Email = contact.Email;
                org.Phone = contact.PhoneNumber;
                return org;
            }   
        }

        private async Task<AssessmentOrganisationContact> GetPrimaryOrFirstContact(string organisationId)
        {
            var contacts = await GetAssessmentOrganisationContacts(organisationId);

            var assessmentOrganisationContacts = contacts as IList<AssessmentOrganisationContact> ?? contacts.ToList();
            AssessmentOrganisationContact contact;

            if (assessmentOrganisationContacts.Where(x => x.IsPrimaryContact == true).Count() > 0)
                contact = assessmentOrganisationContacts.Where(x => x.IsPrimaryContact == true).First();
            else
                contact = assessmentOrganisationContacts.FirstOrDefault();
            return contact;
        }

        public async Task<IEnumerable<AssessmentOrganisationAddress>> GetAssessmentOrganisationAddresses(string organisationId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();


                var sqlForAddress =
                    "SELECT JSON_VALUE(OrganisationData, '$.Address1') AS[Primary], " +
                    "JSON_VALUE(OrganisationData, '$.Address2') AS[Secondary], " +
                    "JSON_VALUE(OrganisationData, '$.Address3') AS Street, " +
                    "JSON_VALUE(OrganisationData, '$.Address4') AS[Town], " +
                    "JSON_VALUE(OrganisationData, '$.Postcode') AS Postcode " +
                    $@"FROM [Organisations] where EndPointAssessorOrganisationId = '{organisationId}'";

                return await connection.QueryAsync<AssessmentOrganisationAddress>(sqlForAddress);
            }
        }

        public async Task<IEnumerable<AssessmentOrganisationContact>> GetAssessmentOrganisationContacts(string organisationId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql =   "SELECT C.Id, C.EndPointAssessorOrganisationId as OrganisationId, C.CreatedAt, C.DeletedAt, " +
                            "C.DisplayName, C.email, C.Status, C.UpdatedAt, C.Username, C.PhoneNumber, " + 
                            "CASE PrimaryContact WHEN NULL THEN 0 ELSE 1 END AS IsPrimaryContact " +
                            "from contacts C  left outer join Organisations O on " + 
                            "C.Username = O.PrimaryContact AND C.EndPointAssessorOrganisationId = O.EndPointAssessorOrganisationId " +
                            $@"where C.EndPointAssessorOrganisationId = '{organisationId}'";

                return await connection.QueryAsync<AssessmentOrganisationContact>(sql);
            }
        }
    }
}
