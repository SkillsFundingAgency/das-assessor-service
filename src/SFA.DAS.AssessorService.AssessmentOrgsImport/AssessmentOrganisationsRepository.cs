using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using SFA.DAS.AssessorService.AssessmentOrgsImport.models;
using SFA.DAS.AssessorService.Data;

namespace SFA.DAS.AssessorService.AssessmentOrgsImport
{
    public class AssessmentOrganisationsRepository
    {
        

        private readonly string _connectionString = ConfigurationWrapper.AccessorDbConnectionString;

   
        public void TearDownData()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                
                connection.Execute("DELETE FROM [OrganisationStandardDeliveryArea]");
                connection.Execute("DELETE FROM [OrganisationStandard]");
                connection.Execute("DELETE FROM [OrganisationType]");
                connection.Execute("DELETE FROM [DeliveryArea]");
                connection.Execute("delete from contacts where username like 'unknown%'");
                connection.Execute("delete from organisations where  status = 'New'");   // and check not in contacts
                connection.Close();
            }
        }

        public List<DeliveryArea> WriteDeliveryAreas(List<DeliveryArea> deliveryAreas)
        {

            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var currentNumber = connection.ExecuteScalar("select count(0) from [DeliveryArea]").ToString();
                if (currentNumber == "0")
                {
                    connection.Execute("INSERT INTO [DeliveryArea] ([Area],[Status]) VALUES (@Area, @Status)", deliveryAreas);
                 }
                var delivAreas = connection.Query<DeliveryArea>("select * from [DeliveryArea]").ToList();

                connection.Close();

                return delivAreas;
            }
        }
        
        public List<TypeOfOrganisation> WriteOrganisationTypes(List<TypeOfOrganisation> organisationTypes)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var currentNumber = connection.ExecuteScalar("select count(0) from [OrganisationType]").ToString();
                if (currentNumber == "0")
                {
                    connection.Execute("INSERT INTO [OrganisationType] ([OrganisationType], [Status]) VALUES (@OrganisationType, @Status)", organisationTypes);
                }

                var orgTypes = connection.Query<TypeOfOrganisation>("select * from [OrganisationType]").ToList();
                connection.Close();

                return orgTypes;
            }
        }

        public List<EpaOrganisation> WriteOrganisations(List<EpaOrganisation> organisations)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                foreach (var organisation in organisations)
                {
                    var currentCount = connection.ExecuteScalar("select count(0) from [Organisations] where EndPointAssessorOrganisationId = @EndPointAssessorOrganisationId", organisation)
                        .ToString();
                    if (currentCount == "0")
                    {
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
                            organisation);
                    }
                }
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
            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var currentNumber = connection.ExecuteScalar("select count(0) from [OrganisationStandard]").ToString();
                if (currentNumber == "0")
                {

                    
                        connection.Execute(@"INSERT INTO [OrganisationStandard]
                                       ([EndPointAssessorOrganisationId]
                                       ,[StandardCode]
                                       ,[EffectiveFrom]
                                       ,[EffectiveTo]
                                       ,[DateStandardApprovedOnRegister]
                                       ,[Comments]
                                       ,[Status])
                                 VALUES
                                       (@EndPointAssessorOrganisationId
                                       ,@StandardCode
                                       ,@EffectiveFrom
                                       ,@EffectiveTo
                                       ,@DateStandardApprovedOnRegister
                                       ,@Comments,
                                        @Status)",
                            orgStandards);
                    
                }
                   connection.Close();
            }

        }

        public void WriteStandardDeliveryAreas(List<EpaOrganisationStandardDeliveryArea> organisationStandardDeliveryAreas)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var currentNumber = connection.ExecuteScalar("select count(0) from [OrganisationStandardDeliveryArea]").ToString();
                if (currentNumber == "0")
                {
                    connection.Execute(@"INSERT INTO [OrganisationStandardDeliveryArea]
                                           ([EndPointAssessorOrganisationId]
                                           ,[StandardCode]
                                           ,[DeliveryAreaId]
                                           ,[Comments]
                                            ,[Status])
                                     VALUES
                                           (@EndPointAssessorOrganisationId
                                           ,@StandardCode
                                           ,@DeliveryAreaId
                                           ,@Comments
                                            ,@Status)",
                                            organisationStandardDeliveryAreas);
                }

                connection.Close();
            }

        }

        public void WriteOrganisationContacts(List<OrganisationContact> contacts)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                foreach (var contact in contacts)
                {
                    var numberOfMatches = connection.ExecuteScalar("select count(0) from [Contacts] where [EndPointAssessorOrganisationId] = @EndPointAssessorOrganisationId and email = @email", contact).ToString();
                    if (numberOfMatches == "0")
                    {
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
                                                contact);
                    }
                }
                connection.Close();
            }
        }
    }

}
