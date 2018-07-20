using System;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities.AssessmentOrganisations;

namespace SFA.DAS.AssessorService.Data
{
    public class AssessmentOrgsRepository : IAssessmentOrgsRepository
    {
        private readonly IConfigurationWrapper _configurationWrapper;
        private readonly ILogger<AssessmentOrgsRepository> _logger;
        private bool UseStringBuilder = true;

        public AssessmentOrgsRepository(IConfigurationWrapper configurationWrapper, ILogger<AssessmentOrgsRepository> logger)
        {
            _configurationWrapper = configurationWrapper;
            _logger = logger;
        }

        public void SetBuildAction(bool useStringBuilder)
        {
            UseStringBuilder = useStringBuilder;
        }

        public string TearDownData()
        {
            var progressStatus = new StringBuilder();
            try
            {

                var connectionString = _configurationWrapper.DbConnectionString;

                progressStatus.Append("Teardown: Opening connection to database; ");
                using (var connection = new SqlConnection(connectionString))
                {
                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    progressStatus.Append("Teardown: DELETING all items in [OrganisationStandardDeliveryArea]; ");
                    connection.Execute("DELETE FROM [OrganisationStandardDeliveryArea]");
                    progressStatus.Append("Teardown: DELETING all items in [OrganisationStandard]; ");
                    connection.Execute("DELETE FROM [OrganisationStandard]");
                    progressStatus.Append("Teardown: DELETING all items in [DeliveryArea]; ");
                    connection.Execute("DELETE FROM [DeliveryArea]");
                    progressStatus.Append("Teardown: DELETING selected [Contacts]; ");
                    connection.Execute("DELETE FROM [contacts] WHERE username LIKE 'unknown%'");
                    progressStatus.Append("Teardown: DELETING selected [Organisations]; ");
                    connection.Execute("DELETE FROM [organisations] where  status = 'New' and Id not in (select organisationid from [contacts])");
                    progressStatus.Append("Teardown: DELETING selected [OrganisationType]; ");
                    connection.Execute("DELETE FROM [OrganisationType] where id not in (select organisationtypeid from [organisations] where organisationtypeid is not null)");
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                progressStatus.Append("Teardown: DELETION Error; ");
                _logger.LogError($"Progress status: {progressStatus}",e);

                throw;
            }

            progressStatus.Append("Teardown: Complete; ");
            _logger.LogInformation($"Progress status: {progressStatus}");

            return progressStatus.ToString();

        }

        public void WriteDeliveryAreas(List<DeliveryArea> deliveryAreas)
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
                    connection.Execute(
                        "set identity_insert [DeliveryArea] ON; INSERT INTO [DeliveryArea] ([id], [Area],[Status]) VALUES (@Id, @Area, @Status); set identity_insert[DeliveryArea] OFF; ",
                        deliveryAreas, transaction);
                    transaction.Commit();
                }

                connection.Close();
            }
        }

        public void WriteOrganisationTypes(List<TypeOfOrganisation> organisationTypes)
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
                    "set identity_insert [OrganisationType] ON; INSERT INTO [OrganisationType] (Id, [OrganisationType], [Status]) VALUES (@Id, @OrganisationType, @Status); set identity_insert [OrganisationType] OFF; ",
                    organisationTypesToInsert, transaction);
                transaction.Commit();
             
               connection.Close();
            }
        }

        public void WriteOrganisations(List<EpaOrganisation> organisations)
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
                            "select count(0) from [Organisations] where EndPointAssessorOrganisationId = @EndPointAssessorOrganisationId", organisation)
                            .ToString();
                    if (currentCount == "0")
                    {
                        organisationsToInsert.Add(organisation);
                    }
                }

                if (UseStringBuilder)
                {
                    var sql = new StringBuilder();

                    foreach (var org in organisationsToInsert)
                    {
                        var id = ConvertStringToSqlValueString(org.Id.ToString());
                        var endPointAssessorName = ConvertStringToSqlValueString(org.EndPointAssessorName);
                        var websiteLink = ConvertStringToSqlValueString(org.WebsiteLink);
                        var legalName = ConvertStringToSqlValueString(org.LegalName);
                        var address1 = ConvertStringToSqlValueString(org.Address1);
                        var address2 = ConvertStringToSqlValueString(org.Address2);
                        var address3 = ConvertStringToSqlValueString(org.Address3);
                        var address4 = ConvertStringToSqlValueString(org.Address4);
                        var postcode = ConvertStringToSqlValueString(org.Postcode);
                        var ukprn = ConvertIntToSqlValueString(org.EndPointAssessorUkprn);

                        sql.Append(
                            "INSERT INTO [Organisations] ([Id] ,[CreatedAt] ,[DeletedAt],[EndPointAssessorName] ,[EndPointAssessorOrganisationId], " +
                            "[EndPointAssessorUkprn],[PrimaryContact],[Status],[UpdatedAt],[WebsiteLink],[OrganisationTypeId],[LegalName], Address1, Address2," +
                            " Address3, Address4, Postcode) VALUES (" +
                            $@" {id}, getdate(), null, {endPointAssessorName}, '{
                                    org.EndPointAssessorOrganisationId
                                }'," +
                            $@"{ukprn}, null, '{org.Status}', null, {websiteLink}, {
                                    org.OrganisationTypeId
                                }, {legalName}, {address1}, {address2}, " +
                            $@"{address3}, {address4}, {postcode} ); ");
                    }
                    connection.Execute(sql.ToString());
                }
                else
                {


                    IDbTransaction transaction = connection.BeginTransaction();
                    connection.Execute(@"INSERT INTO [Organisations]
                                            ([Id]
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
                                             (@Id
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
                }
              
                connection.Close();
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
                    if (UseStringBuilder)
                    {
                        foreach (var organisationStandard in orgStandards)
                        {

                            var comments = ConvertStringToSqlValueString(organisationStandard.Comments);
                            var effectiveFrom = ConvertDateToSqlValueString(organisationStandard.EffectiveFrom);
                            var effectiveTo = ConvertDateToSqlValueString(organisationStandard.EffectiveTo);
                            var dateStandardApprovedOnRegister =
                                ConvertDateToSqlValueString(organisationStandard.DateStandardApprovedOnRegister);


                            sql.Append(
                                "INSERT INTO [OrganisationStandard] ([EndPointAssessorOrganisationId],[StandardCode],[EffectiveFrom],[EffectiveTo],[DateStandardApprovedOnRegister],[Comments],[Status])" +
                                $"VALUES ('{organisationStandard.EndPointAssessorOrganisationId}' ,'{organisationStandard.StandardCode}' ,{effectiveFrom} ,{effectiveTo} ,{dateStandardApprovedOnRegister} ,{comments} ,'{organisationStandard.Status}'); ");
                        }
                        connection.Execute(sql.ToString());
                    }
                    else
                    {
                        IDbTransaction transaction = connection.BeginTransaction();
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
                           orgStandards, transaction);
                        transaction.Commit();
                    }

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
                    if (UseStringBuilder)
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
                    }
                    else
                    {
                        IDbTransaction transaction = connection.BeginTransaction();
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
                            organisationStandardDeliveryAreas, transaction);
                        transaction.Commit();
                    }
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
                if (UseStringBuilder)
                {
                    var sql = new StringBuilder();

                    foreach (var contact in contactsToInsert)
                    {

                        var displayName = ConvertStringToSqlValueString(contact.DisplayName);
                        var email = ConvertStringToSqlValueString(contact.Email);
                        var endPointAssessorOrganisationId =
                            ConvertStringToSqlValueString(contact.EndPointAssessorOrganisationId);
                        var organisationId = ConvertStringToSqlValueString(contact.OrganisationId.ToString());
                        var userName = ConvertStringToSqlValueString(contact.Username);
                        var phoneNumber = ConvertStringToSqlValueString(contact.PhoneNumber);


                        sql.Append(
                            "INSERT INTO [Contacts] ([Id] ,[CreatedAt] ,[DeletedAt] ,[DisplayName] ,[Email] ,[EndPointAssessorOrganisationId] ,[OrganisationId],  " +
                            "[Status], [UpdatedAt], [Username] ,[PhoneNumber]) VALUES " +
                            $@"(newid(), getdate(), null, {displayName}, {email}, {endPointAssessorOrganisationId}, {
                                    organisationId
                                }, " +
                            $@"'{contact.Status}', getdate(), {userName}, {phoneNumber}); ");
                    }
                    connection.Execute(sql.ToString());

                }
                else
                {
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
                }
                connection.Close();
            }
        }

        private static string ConvertStringToSqlValueString(string stringToProcess)
        {
            return stringToProcess == null
                ? "null"
                : $@"'{stringToProcess.Replace("'","''")}'";
        }

        private static string ConvertIntToSqlValueString(int? intToProcess)
        {
            return intToProcess == null
                ? "null"
                : $@"{intToProcess}";
        }

        private static string  ConvertDateToSqlValueString (DateTime? dateToProcess)
        {           
                return dateToProcess == null
                    ? "null"
                    : $"'{dateToProcess.Value:yyyy-MM-dd}'";       
        }

       
    }
}
