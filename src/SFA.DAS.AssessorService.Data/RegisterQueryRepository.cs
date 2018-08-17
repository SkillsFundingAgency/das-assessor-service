﻿using SFA.DAS.AssessorService.Application.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System;
using System.Linq;

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

        public async Task<bool> EpaOrganisationExistsWithOrganisationId(string organisationId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                var sqlToCheckExists =
                    "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Organisations] " +
                    $@"WHERE EndPointAssessorOrganisationId = '{organisationId}'";
                return await connection.ExecuteScalarAsync<bool>(sqlToCheckExists);
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
        
        public async Task<bool> EpaOrganisationExistsWithUkprn(long ukprn)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                var sqlToCheckExists =
                    "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Organisations] " +
                    $@"WHERE EndPointAssessorUkprn = {ukprn}";
                return await connection.ExecuteScalarAsync<bool>(sqlToCheckExists);
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
                    $@"WHERE Id = {organisationTypeId}";
                return await connection.ExecuteScalarAsync<bool>(sqlToCheckExists);
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
                    $@"WHERE Id = '{id}'";
                var orgs = await connection.QueryAsync<EpaOrganisation>(sqlForMainDetails);
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
                    $@"WHERE EndPointAssessorOrganisationId = '{organisationId}'";
                var orgs = await connection.QueryAsync<EpaOrganisation>(sqlForMainDetails);
                var org = orgs.FirstOrDefault();
                return org;
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
                    $@"WHERE EndPointAssessorOrganisationId != '{organisationIdToExclude}' and EndPointAssessorUkprn = {ukprn}";
                return await connection.ExecuteScalarAsync<bool>(sqlToCheckExists);
            }
        }
    }
}
