﻿using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Settings;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;
using System.Linq;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using SFA.DAS.AssessorService.Application.Exceptions;

namespace SFA.DAS.AssessorService.Data
{
    public class RegisterRepository : IRegisterRepository
    {

        private readonly IWebConfiguration _configuration;

        public RegisterRepository(IWebConfiguration configuration)
        {
            _configuration = configuration;
            SqlMapper.AddTypeHandler(typeof(OrganisationData), new OrganisationDataHandler());
        }

        public async Task<string> CreateEpaOrganisation(EpaOrganisation org)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var orgData = JsonConvert.SerializeObject(org.OrganisationData);

                connection.Execute(
                    "INSERT INTO [Organisations] ([Id],[CreatedAt],[EndPointAssessorName],[EndPointAssessorOrganisationId], " +
                    "[EndPointAssessorUkprn],[Status],[OrganisationTypeId],[OrganisationData]) " +
                    $@"VALUES (@id, getdate(), @name, @organisationId, @ukprn, 'New', @organisationTypeId,  @orgData)",
                    new {org.Id, org.Name, org.OrganisationId, org.Ukprn, org.Status, org.OrganisationTypeId, orgData}
                );

                return org.OrganisationId;

            }
        }

        public async Task<string> UpdateEpaOrganisation(EpaOrganisation org)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var orgData = JsonConvert.SerializeObject(org.OrganisationData);

                connection.Execute(
                    "UPDATE [Organisations] SET [UpdatedAt] = GetDate(), [EndPointAssessorName] = @Name, " +
                    "[EndPointAssessorUkprn] = @ukprn, [OrganisationTypeId] = @organisationTypeId, " +
                    "[OrganisationData] = @orgData WHERE [EndPointAssessorOrganisationId] = @organisationId",
                    new {org.Name, org.Ukprn, org.OrganisationTypeId, orgData, org.OrganisationId});
       
                return org.OrganisationId;
            }
        }

        public async Task<int>CreateEpaOrganisationStandard(EpaOrganisationStandard organisationStandard)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

             
                var res = connection.Query<int>(
                    "INSERT INTO [dbo].[OrganisationStandard] ([EndPointAssessorOrganisationId],[StandardCode],[EffectiveFrom],[EffectiveTo],[DateStandardApprovedOnRegister] ,[Comments],[Status]) VALUES (" +
                    "@organisationId, @standardcode, @effectiveFrom, @effectiveTo, @dateStandardApprovedOnRegister, @comments, 'New'); SELECT CAST(SCOPE_IDENTITY() as int); ",
                    new
                    {
                        organisationStandard.OrganisationId, organisationStandard.StandardCode, organisationStandard.EffectiveFrom, organisationStandard.EffectiveTo,
                        organisationStandard.DateStandardApprovedOnRegister, organisationStandard.Comments}).Single();

                return res;
            }
        }

        public async Task<int> UpdateEpaOrganisationStandard(EpaOrganisationStandard orgStandard)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var res = connection.Query<int>(
                    "UPDATE [OrganisationStandard] SET [EffectiveFrom] = @effectiveFrom, [EffectiveTo] = @EffectiveTo, " +
                    "[DateStandardApprovedOnRegister] = @dateStandardApprovedOnRegister, [Comments] = @comments " +
                    "WHERE [EndPointAssessorOrganisationId] = @organisationId and [StandardCode] = @standardCode; SELECT top 1 id from [organisationStandard] where  [EndPointAssessorOrganisationId] = @organisationId and [StandardCode] = @standardCode;",
                    new {orgStandard.EffectiveFrom, orgStandard.EffectiveTo,orgStandard.DateStandardApprovedOnRegister, orgStandard.Comments, orgStandard.OrganisationId, orgStandard.StandardCode}).Single();

                return res;
            }
        }
    }
}
