using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Data;
using System.Data.SqlClient;
using AutoMapper.Configuration;
using Dapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Data.Configuration;
using SFA.DAS.AssessorService.Api.Types.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFA.DAS.AssessorService.Domain.Entities.AssessmentOrganisations;

namespace SFA.DAS.AssessorService.Data
{
    public class AssessmentOrgsRepository : IAssessmentOrgsRepository
    {
        private readonly IConfigurationWrapper _configurationWrapper;

        public AssessmentOrgsRepository(IConfigurationWrapper configurationWrapper)
        {
            _configurationWrapper = configurationWrapper;
        }


        public void TearDownData()
        {
            try
            {

                var connectionString = _configurationWrapper.DbConnectionString;

                using (var connection = new SqlConnection(connectionString))
                {
                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    connection.Execute("DELETE FROM [OrganisationStandardDeliveryArea]");
                    connection.Execute("DELETE FROM [OrganisationStandard]");
                    connection.Execute("DELETE FROM [DeliveryArea]");
                    connection.Execute("DELETE FROM [contacts] WHERE username LIKE 'unknown%'");
                    connection.Execute(
                        "DELETE FROM [organisations] where  status = 'New' and Id not in (select organisationid from [contacts])");
                    connection.Execute(
                        "DELETE FROM [OrganisationType] where id not in (select organisationtypeid from [organisations] where organisationtypeid is not null)");
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public List<DeliveryArea> WriteDeliveryAreas(List<DeliveryArea> deliveryAreas)
        {
            var connectionString = _configurationWrapper.DbConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var currentNumber = connection.ExecuteScalar("select count(0) from [DeliveryArea]").ToString();
                if (currentNumber == "0")
                {
                    IDbTransaction transaction = connection.BeginTransaction();
                    connection.Execute("INSERT INTO [DeliveryArea] ([Area],[Status]) VALUES (@Area, @Status)",
                        deliveryAreas, transaction);
                    transaction.Commit();
                }
                var delivAreas = connection.Query<DeliveryArea>("select * from [DeliveryArea]").ToList();

                connection.Close();

                return delivAreas;
            }
        }

        public List<TypeOfOrganisation> WriteOrganisationTypes(List<TypeOfOrganisation> organisationTypes)
        {
            var connectionString = _configurationWrapper.DbConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();


                var organisationTypesToInsert = new List<TypeOfOrganisation>();

                foreach (var organisationType in organisationTypes)
                {
                    var currentNumber = connection
                        .ExecuteScalar(
                            "select count(0) from [OrganisationType] where OrganisationType = @OrganisationType",
                            organisationType).ToString();
                    if (currentNumber == "0")
                    {
                        organisationTypesToInsert.Add(organisationType);
                    }
                }

                IDbTransaction transaction = connection.BeginTransaction();
                connection.Execute(
                    "INSERT INTO [OrganisationType] ([OrganisationType], [Status]) VALUES (@OrganisationType, @Status)",
                    organisationTypesToInsert, transaction);
                transaction.Commit();

                var orgTypes = connection.Query<TypeOfOrganisation>("select * from [OrganisationType]").ToList();
                connection.Close();

                return orgTypes;
            }
        }

        public List<EpaOrganisation> WriteOrganisations(List<EpaOrganisation> organisations)
        {
            var connectionString = _configurationWrapper.DbConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var organisationsToInsert = new List<EpaOrganisation>();

                foreach (var organisation in organisations)
                {
                    var currentCount = connection
                        .ExecuteScalar(
                            "select count(0) from [Organisations] where EndPointAssessorOrganisationId = @EndPointAssessorOrganisationId",
                            organisation)
                        .ToString();
                    if (currentCount == "0")
                    {
                        organisationsToInsert.Add(organisation);
                    }
                }

                IDbTransaction transaction = connection.BeginTransaction();
                connection.Execute(@"INSERT INTO [Organisations]
                                        (
                                        [Id]
                                       ,[CreatedAt]
                                       ,[DeletedAt]
                                       ,[EndPointAssessorName]
                                       ,[EndPointAssessorOrganisationId]
                                       ,[EndPointAssessorUkprn]
                                       ,[PrimaryContact]
                                       ,[Status]
                                       ,[UpdatedAt]
                                       ,[WebsiteLink]
                                       ,[OrganisationTypeId]
                                       ,[LegalName]
                                        ,Address1
                                        ,Address2
                                        ,Address3
                                        ,Address4
                                        ,Postcode
                                        )
                                    VALUES
                                         (
                                        newid()
                                        ,getdate()
                                        ,null
                                        ,@EndPointAssessorName
                                        ,@EndPointAssessorOrganisationId
                                        ,@EndPointAssessorUkprn
                                        ,null
                                        ,@Status
                                        ,null
                                        ,@WebsiteLink
                                        ,@OrganisationTypeId
                                        ,@LegalName 
                                        ,@Address1
                                        ,@Address2
                                        ,@Address3
                                        ,@Address4
                                        ,@Postcode)",
                    organisationsToInsert, transaction);
                    transaction.Commit();

                var orgs = connection.Query<EpaOrganisation>("select * from [Organisations]").ToList();

                connection.Close();

                foreach (var org in orgs)
                {
                    var matchingOrg = organisations
                        .FirstOrDefault(x => x.EndPointAssessorOrganisationId == org.EndPointAssessorOrganisationId);

                    org.Address1 = matchingOrg?.Address1;
                    org.Address2 = matchingOrg?.Address2;
                    org.Address3 = matchingOrg?.Address3;
                    org.Address4 = matchingOrg?.Address4;
                    org.Postcode = matchingOrg?.Postcode;
                }

                return orgs;
            }
        }

        public void WriteEpaOrganisationStandards(List<EpaOrganisationStandard> orgStandards)
        {
            var connectionString = _configurationWrapper.DbConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                var sql = new StringBuilder();

                var currentNumber = connection.ExecuteScalar("select count(0) from [OrganisationStandard]").ToString();
                if (currentNumber == "0")
                {
                    foreach (var organisationStandard in orgStandards)
                    {

                        var comments = ConvertStringToSqlValueString(organisationStandard.Comments);
                        var effectiveFrom = ConvertDateToSqlValueString(organisationStandard.EffectiveFrom);
                        var effectiveTo = ConvertDateToSqlValueString(organisationStandard.EffectiveTo);
                        var dateStandardApprovedOnRegister =
                            ConvertDateToSqlValueString(organisationStandard.DateStandardApprovedOnRegister);


                        sql.Append("INSERT INTO [OrganisationStandard] ([EndPointAssessorOrganisationId],[StandardCode],[EffectiveFrom],[EffectiveTo],[DateStandardApprovedOnRegister],[Comments],[Status])" +
                            $"VALUES ('{organisationStandard.EndPointAssessorOrganisationId}' ,'{organisationStandard.StandardCode}' ,{effectiveFrom} ,{effectiveTo} ,{dateStandardApprovedOnRegister} ,{comments} ,'{organisationStandard.Status}'); ");
                    }
                    connection.Execute(sql.ToString());

                    //IDbTransaction transaction = connection.BeginTransaction();
                    //connection.Execute(@"INSERT INTO [OrganisationStandard]
                    //                   ([EndPointAssessorOrganisationId]
                    //                   ,[StandardCode]
                    //                   ,[EffectiveFrom]
                    //                   ,[EffectiveTo]
                    //                   ,[DateStandardApprovedOnRegister]
                    //                   ,[Comments]
                    //                   ,[Status])
                    //             VALUES
                    //                   (@EndPointAssessorOrganisationId
                    //                   ,@StandardCode
                    //                   ,@EffectiveFrom
                    //                   ,@EffectiveTo
                    //                   ,@DateStandardApprovedOnRegister
                    //                   ,@Comments,
                    //                    @Status)",
                    //   orgStandards,transaction);
                    //transaction.Commit();


                    connection.Close();
                }
            }
        }

      

        public void WriteStandardDeliveryAreas(
            List<EpaOrganisationStandardDeliveryArea> organisationStandardDeliveryAreas)
        {
            var connectionString = _configurationWrapper.DbConnectionString;
            var sql = new StringBuilder();
            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var currentNumber = connection.ExecuteScalar("select count(0) from [OrganisationStandardDeliveryArea]")
                    .ToString();
                if (currentNumber == "0")
                {
                    foreach (var organisationStandardDeliveryArea in organisationStandardDeliveryAreas)
                    {
                        sql.Append($@"INSERT INTO [OrganisationStandardDeliveryArea]
                                           ([EndPointAssessorOrganisationId]
                                           ,[StandardCode]
                                           ,[DeliveryAreaId]
                                           ,[Comments]
                                            ,[Status])
                                     VALUES
                                           ('{organisationStandardDeliveryArea.EndPointAssessorOrganisationId}'
                                           , '{organisationStandardDeliveryArea.StandardCode}'
                                           , {organisationStandardDeliveryArea.DeliveryAreaId}
                                           , '{organisationStandardDeliveryArea.Comments}'
                                            , '{organisationStandardDeliveryArea.Status}'); ");

                    }
                    connection.Execute(sql.ToString());

                    //IDbTransaction transaction = connection.BeginTransaction();
                    //connection.Execute(@"INSERT INTO [OrganisationStandardDeliveryArea]
                    //                       ([EndPointAssessorOrganisationId]
                    //                       ,[StandardCode]
                    //                       ,[DeliveryAreaId]
                    //                       ,[Comments]
                    //                        ,[Status])
                    //                 VALUES
                    //                       (@EndPointAssessorOrganisationId
                    //                       ,@StandardCode
                    //                       ,@DeliveryAreaId
                    //                       ,@Comments
                    //                        ,@Status)",
                    //    organisationStandardDeliveryAreas, transaction);
                    //transaction.Commit();

                }
                connection.Close();
            }

        }

        public void WriteOrganisationContacts(List<OrganisationContact> contacts)
        {
            var connectionString = _configurationWrapper.DbConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var contactsToInsert = new List<OrganisationContact>();
                foreach (var contact in contacts)
                {
                    var numberOfMatches = connection
                        .ExecuteScalar(
                            "select count(0) from [Contacts] where [EndPointAssessorOrganisationId] = @EndPointAssessorOrganisationId and email = @email",
                            contact).ToString();
                    if (numberOfMatches == "0")
                    {
                        contactsToInsert.Add(contact);
                    }
                }

                IDbTransaction transaction = connection.BeginTransaction();
                connection.Execute(@"INSERT INTO [Contacts]
                                                ([Id]
                                                   ,[CreatedAt]
                                                   ,[DeletedAt]
                                                   ,[DisplayName]
                                                   ,[Email]
                                                   ,[EndPointAssessorOrganisationId]
                                                   ,[OrganisationId]
                                                   ,[Status]
                                                   ,[UpdatedAt]
                                                   ,[Username]
                                                   ,[PhoneNumber]
                                                   )
                                         VALUES
                                               (newid()
                                               ,getdate()
                                               ,null
                                               ,@DisplayName
                                               ,@Email
                                               ,@EndPointAssessorOrganisationId
                                               ,@OrganisationId
                                               ,@Status
                                               ,getdate()
                                               ,@Username
                                               ,@PhoneNumber)",
                    contactsToInsert, transaction);
                transaction.Commit();

                connection.Close();
            }
        }

        private static string ConvertStringToSqlValueString(string stringToProcess)
        {
            return stringToProcess == null
                ? "null"
                : $@"'{stringToProcess.Replace("'","''")}'";
        }

        private static string  ConvertDateToSqlValueString (DateTime? dateToProcess)
        {           
                return dateToProcess == null
                    ? "null"
                    : $"'{dateToProcess.Value:yyyy-MM-dd}'";       
        }
    }
}
