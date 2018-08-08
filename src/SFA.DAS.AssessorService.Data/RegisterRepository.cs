using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Settings;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Mime;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using SFA.DAS.AssessorService.Data.Services;
using SFA.DAS.AssessorService.Application.Exceptions;

namespace SFA.DAS.AssessorService.Data
{
    public class RegisterRepository : IRegisterRepository
    {
        private readonly ISqlStringProcessingService _stringProcessingService;

        private readonly IWebConfiguration _configuration;
        public RegisterRepository(IWebConfiguration configuration, ISqlStringProcessingService stringProcessingService)
        {
            _configuration = configuration;
            _stringProcessingService = stringProcessingService;

            SqlMapper.AddTypeHandler(typeof(OrganisationData), new OrganisationDataHandler());
        }

        public async Task<EpaOrganisation> CreateEpaOrganisation(EpaOrganisation organisation)
        {

            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                var organisationData = new OrganisationData
                {
                    WebsiteLink = _stringProcessingService.MakeStringSuitableForJson(organisation.OrganisationData?.WebsiteLink),
                    LegalName = _stringProcessingService.MakeStringSuitableForJson(organisation.OrganisationData?.LegalName),
                    Address1 = _stringProcessingService.MakeStringSuitableForJson(organisation.OrganisationData?.Address1),
                    Address2 = _stringProcessingService.MakeStringSuitableForJson(organisation.OrganisationData?.Address2),
                    Address3 = _stringProcessingService.MakeStringSuitableForJson(organisation.OrganisationData?.Address3),
                    Address4 = _stringProcessingService.MakeStringSuitableForJson(organisation.OrganisationData?.Address4),
                    Postcode = _stringProcessingService.MakeStringSuitableForJson(organisation.OrganisationData?.Postcode)
                };

                var orgData = JsonConvert.SerializeObject(organisationData);

                var id = _stringProcessingService.ConvertStringToSqlValueString(organisation.Id.ToString());
                var ukprn = _stringProcessingService.ConvertLongToSqlValueString(organisation.Ukprn);
                var name = _stringProcessingService.ConvertStringToSqlValueString(organisation.Name);
                var organisationTypeId = _stringProcessingService.ConvertIntToSqlValueString(organisation.OrganisationTypeId);
                var organisationId = _stringProcessingService.ConvertStringToSqlValueString(organisation.OrganisationId);
                var sqlToInsert =
                    "INSERT INTO [Organisations] ([Id],[CreatedAt],[EndPointAssessorName],[EndPointAssessorOrganisationId], " +
                    "[EndPointAssessorUkprn],[Status],[OrganisationTypeId],[OrganisationData]) " +
                    $@"VALUES ({id}, getdate(), {name}, {organisationId},{ukprn}, '{organisation.Status}', {organisationTypeId}, " +
                    $@"'{orgData}')";

                var organisationAlreadyExists = await GetEpaOrganisationByOrganisationId(organisation.OrganisationId);
                if (organisationAlreadyExists != null)
                    throw new AlreadyExistsException($@"There is already an entry for [{organisation.OrganisationId}]");

                var res = connection.Execute(sqlToInsert);

                if (res == 1)
                    return await GetEpaOrganisationById(organisation.Id);
            }

            return null;
        }

        public async Task<EpaOrganisation> GetEpaOrganisationById(Guid epaOrganisationId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                var sqlForMainDetails =
                    "select Id, CreatedAt, DeletedAt, EndPointAssessorName as Name,  EndPointAssessorOrganisationId as OrganisationId, EndPointAssessorUkprn as ukprn, " +
                    "primaryContact, Status, UpdatedAt, OrganisationTypeId, OrganisationData " +
                    " FROM [Organisations] " +
                    $@"WHERE Id = '{epaOrganisationId}'";
                var orgs = await connection.QueryAsync<EpaOrganisation>(sqlForMainDetails);
                var org = orgs.FirstOrDefault();
                return org;
            }
        }

        public async Task<EpaOrganisation> GetEpaOrganisationByOrganisationId(int organisationId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                var sqlForMainDetails =
                    "select Id, CreatedAt, DeletedAt, EndPointAssessorName as Name,  EndPointAssessorOrganisationId as OrganisationId, EndPointAssessorUkprn as ukprn, " +
                    "primaryContact, Status, UpdatedAt, OrganisationTypeId, OrganisationData " +
                    " FROM [Organisations] " +
                    $@"WHERE EndPointAssessorOrganisationId = '{organisationId}'";
                var orgs = await connection.QueryAsync<EpaOrganisation>(sqlForMainDetails);
                var org = orgs.FirstOrDefault();
                return org;
            }
        }
    }
}
