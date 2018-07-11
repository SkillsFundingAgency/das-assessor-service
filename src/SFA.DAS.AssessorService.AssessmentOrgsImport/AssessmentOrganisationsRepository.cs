using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using SFA.DAS.AssessorService.AssessmentOrgsImport.models;

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
                
                connection.Execute("DELETE FROM [ao].[EpaOrganisationStandardDeliveryArea]");
                connection.Execute("DELETE FROM [ao].[EPAOrganisationStandard]");
                connection.Execute("DELETE FROM [ao].[EPAOrganisation]");
                connection.Execute("DELETE FROM [ao].[OrganisationType]");
                connection.Execute("DELETE FROM [ao].[DeliveryArea]");
                connection.Execute("DELETE FROM [ao].[Standard]");
                connection.Execute("DELETE FROM [ao].[Status]");

                connection.Close();
            }
        }

        public List<DeliveryArea> WriteDeliveryAreas(List<DeliveryArea> deliveryAreas)
        {

            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var currentNumber = connection.ExecuteScalar("select count(0) from [ao].[DeliveryArea]").ToString();
                if (currentNumber == "0")
                {
                    connection.Execute("INSERT INTO [ao].[DeliveryArea] ([Area]) VALUES (@Area)", deliveryAreas);
                 }
                var delivAreas = connection.Query<DeliveryArea>("select * from [ao].[DeliveryArea]").ToList();

                connection.Close();

                return delivAreas;
            }
        }

        public List<Status> WriteStatusCodes()
        {
            var statuses = new List<Status>
            {
                new Status {Id = 1, StatusName = "active"},
                new Status {Id = 2, StatusName = "not released"},
                new Status {Id = 3, StatusName = "deleted"}
            };

            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var currentNumber = connection.ExecuteScalar("select count(0) from [ao].[Status]").ToString();
                if (currentNumber == "0")
                {
                    connection.Execute("INSERT INTO [ao].[Status] ([Id] ,[StatusName]) VALUES (@id, @StatusName)",
                        statuses);
                }
                connection.Close();
            }


            return statuses;
        }

        public List<TypeOfOrganisation> WriteOrganisationTypes(List<TypeOfOrganisation> organisationTypes)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var currentNumber = connection.ExecuteScalar("select count(0) from [ao].[OrganisationType]").ToString();
                if (currentNumber == "0")
                {
                    connection.Execute("INSERT INTO [ao].[OrganisationType] ([OrganisationType]) VALUES (@OrganisationType)", organisationTypes);
                }

                var orgTypes = connection.Query<TypeOfOrganisation>("select * from [ao].[OrganisationType]").ToList();
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

                var currentNumber = connection.ExecuteScalar("select count(0) from [ao].[EPAOrganisation]").ToString();
                if (currentNumber == "0")
                {
                    connection.Execute(@"INSERT INTO [ao].[EPAOrganisation]
                                        (
                                        [EPAOrganisationIdentifier]
                                        ,[EPAOrganisationName]
                                        ,[OrganisationTypeId]
                                        ,[WebsiteLink]
                                        ,[ContactAddress1]
                                        ,[ContactAddress2]
                                        ,[ContactAddress3]
                                        ,[ContactAddress4]
                                        ,[ContactPostcode]
                                        ,[UKPRN]
                                        ,[LegalName]
                                        ,[StatusId])
                                    VALUES
                                         (
                                        @EpaOrganisationIdentifier
                                        ,@EpaOrganisationName
                                        ,@OrganisationTypeId
                                        ,@WebsiteLink
                                        ,@ContactAddress1
                                        ,@ContactAddress2
                                        ,@ContactAddress3
                                        ,@ContactAddress4
                                        ,@ContactPostcode
                                        ,@Ukprn
                                        ,@LegalName
                                        ,@StatusId)", 
                                        organisations);
                }
                var orgs = connection.Query<EpaOrganisation>("select * from [ao].[EPAOrganisation]").ToList();
                connection.Close();

                return orgs;
            }
        }

        public List<Standard> WriteStandards(List<Standard> standards)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var currentNumber = connection.ExecuteScalar("select count(0) from [ao].[Standard]").ToString();
                if (currentNumber == "0")
                {
                   
                        connection.Execute(@"INSERT INTO [ao].[Standard]
                                   ([StandardCode]
                                   ,[Version]
                                   ,[StandardName]
                                   ,[StandardSectorCode]
                                   ,[NotionalEndLevel]
                                   ,[EffectiveFrom]
                                   ,[EffectiveTo]
                                   ,[LastDateStarts]
                                   ,[UrlLink]
                                   ,[SectorSubjectAreaTier1]
                                   ,[SectorSubjectAreaTier2]
                                   ,[IntegratedDegreeStandard]
                                   ,[CreatedOn]
                                   ,[CreatedBy]
                                   ,[ModifiedOn]
                                   ,[ModifiedBy]
                                   ,[StatusId])
                             VALUES
                                   (@StandardCode
                                   ,@Version
                                   ,@StandardName
                                   ,@StandardSectorCode
                                   ,@NotionalEndLevel
                                   ,@EffectiveFrom
                                   ,@EffectiveTo
                                   ,@LastDateStarts
                                   ,@UrlLink
                                   ,@SectorSubjectAreaTier1
                                   ,@SectorSubjectAreaTier2
                                   ,@IntegratedDegreeStandard
                                   ,@CreatedOn
                                   ,@CreatedBy
                                   ,@ModifiedOn
                                   ,@ModifiedBy
                                   ,@StatusId)",
                            standards);
                }

                var standrds = connection.Query<Standard>("select * from [ao].[Standard]").ToList();
                connection.Close();

                return standrds;
            }

        }

        public void WriteEpaOrganisationStandards(List<EpaOrganisationStandard> orgStandards)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var currentNumber = connection.ExecuteScalar("select count(0) from [ao].[EpaOrganisationStandard]").ToString();
                if (currentNumber == "0")
                {

                    foreach (var standard in orgStandards)
                    {
                        connection.Execute(@"INSERT INTO [ao].[EpaOrganisationStandard]
                                       ([EPAOrganisationIdentifier]
                                       ,[StandardCode]
                                       ,[EffectiveFrom]
                                       ,[EffectiveTo]
                                       ,[ContactName]
                                       ,[ContactPhoneNumber]
                                       ,[ContactEmail]
                                       ,[DateStandardApprovedOnRegister]
                                       ,[Comments])
                                 VALUES
                                       (@EPAOrganisationIdentifier
                                       ,@StandardCode
                                       ,@EffectiveFrom
                                       ,@EffectiveTo
                                       ,@ContactName
                                       ,@ContactPhoneNumber
                                       ,@ContactEmail
                                       ,@DateStandardApprovedOnRegister
                                       ,@Comments)",
                            standard);
                    }
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

                var currentNumber = connection.ExecuteScalar("select count(0) from [ao].[EpaOrganisationStandardDeliveryArea]").ToString();
                if (currentNumber == "0")
                {
                    connection.Execute(@"INSERT INTO [ao].[EpaOrganisationStandardDeliveryArea]
                                           ([EPAOrganisationIdentifier]
                                           ,[StandardCode]
                                           ,[DeliveryAreaId]
                                           ,[Comments])
                                     VALUES
                                           (@EPAOrganisationIdentifier
                                           ,@StandardCode
                                           ,@DeliveryAreaId
                                           ,@Comments)",
                                            organisationStandardDeliveryAreas);
                }

                connection.Close();
            }

        }
    }

}
