using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
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
                
                connection.Execute("DELETE FROM [ao].[StandardDeliveryArea]");
                connection.Execute("DELETE FROM [ao].[EPAStandard]");
                connection.Execute("DELETE FROM [ao].[EPAOrganisation]");
                connection.Execute("DELETE FROM [ao].[OrganisationType]");
                connection.Execute("DELETE FROM [ao].[DeliveryArea]");
                connection.Execute("DELETE FROM [ao].[Standard]");

                connection.Close();
            }
        }

        public void WriteDeliveryAreas(List<DeliveryArea> deliveryAreas)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var currentNumber = connection.ExecuteScalar("select count(0) from [ao].[DeliveryArea]").ToString();
                if (currentNumber == "0")
                {
                    connection.Execute("INSERT INTO [ao].[DeliveryArea] ([Id] ,[Area]) VALUES (@id, @Area)", deliveryAreas);
                }
                connection.Close();
            }
        }

        public void WriteOrganisationTypes(List<TypeOfOrganisation> organisationTypes)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var currentNumber = connection.ExecuteScalar("select count(0) from [ao].[OrganisationType]").ToString();
                if (currentNumber == "0")
                {
                    connection.Execute("INSERT INTO [ao].[OrganisationType] ([Id] ,[OrganisationType]) VALUES (@id, @OrganisationType)", organisationTypes);
                }
                connection.Close();
            }
        }

        public void WriteOrganisations(List<EpaOrganisation> organisations)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var currentNumber = connection.ExecuteScalar("select count(0) from [ao].[EPAOrganisation]").ToString();
                if (currentNumber == "0")
                {
                    connection.Execute(@"INSERT INTO [ao].[EPAOrganisation]
                                        ([Id]
                                        ,[EPAOrganisationIdentifier]
                                        ,[EPAOrganisationName]
                                        ,[OrganisationTypeId]
                                        ,[WebsiteLink]
                                        ,[ContactAddress1]
                                        ,[ContactAddress2]
                                        ,[ContactAddress3]
                                        ,[ContactAddress4]
                                        ,[ContactPostcode]
                                        ,[UKPRN]
                                        ,[LegalName])
                                    VALUES
                                        (@Id
                                        ,@EpaOrganisationIdentifier
                                        ,@EpaOrganisationName
                                        ,@OrganisationTypeId
                                        ,@WebsiteLink
                                        ,@ContactAddress1
                                        ,@ContactAddress2
                                        ,@ContactAddress3
                                        ,@ContactAddress4
                                        ,@ContactPostcode
                                        ,@Ukprn
                                        ,@LegalName)", 
                                        organisations);
                }
                connection.Close();
            }
        }

        public void WriteStandards(List<Standard> standards)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var currentNumber = connection.ExecuteScalar("select count(0) from [ao].[Standard]").ToString();
                if (currentNumber == "0")
                {
                   
                        connection.Execute(@"INSERT INTO [ao].[Standard]
                                   ([id]
                                   ,[StandardCode]
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
                                   ,[ModifiedBy])
                             VALUES
                                   (@Id
                                   ,@StandardCode
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
                                   ,@ModifiedBy)",
                            standards);
                }
                connection.Close();
            }

        }

        public void WriteEpaStandards(List<EpaStandard> standards)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var currentNumber = connection.ExecuteScalar("select count(0) from [ao].[EpaStandard]").ToString();
                if (currentNumber == "0")
                {

                    foreach (var standard in standards)
                    {
                        connection.Execute(@"INSERT INTO [ao].[EpaStandard]
                                       ([Id]
                                       ,[EPAOrganisationIdentifier]
                                       ,[StandardCode]
                                       ,[EffectiveFrom]
                                       ,[EffectiveTo]
                                       ,[ContactName]
                                       ,[ContactPhoneNumber]
                                       ,[ContactEmail]
                                       ,[DateStandardApprovedOnRegister]
                                       ,[Comments])
                                 VALUES
                                       (@Id
                                       ,@EPAOrganisationIdentifier
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

    }

}
