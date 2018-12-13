using System;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Transactions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Domain.Entities.AssessmentOrganisations;

namespace SFA.DAS.AssessorService.Data
{
    public class AssessmentOrgsRepository : IAssessmentOrgsRepository
    {
        private readonly IWebConfiguration _configuration;
        private readonly ILogger<AssessmentOrgsRepository> _logger;

        public AssessmentOrgsRepository(IWebConfiguration configuration, ILogger<AssessmentOrgsRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
           
        }

        public string TearDownData()
        {
            var progressStatus = new StringBuilder();
            LogProgress(progressStatus, "Starting TearDownData()");
            try
            {
                var connectionString = LocalConnectionString();
                var obfConnectionString = connectionString.Substring(0,60);
                //connectionString = connectionString.Replace("MultipleActiveResultSets=True", "MultipleActiveResultSets=False");
                if (obfConnectionString.ToLower().Contains("password"))
                    obfConnectionString = "obfuscation full";

                LogProgress(progressStatus, $"Teardown: Opening connection to database with connection string: [{obfConnectionString}]; ");
                using (var connection = new SqlConnection(connectionString))
                {
                    LogProgress(progressStatus, $"Teardown: Using connectionString [{obfConnectionString}], Connection State: [{connection.State}]; ");
                    if (connection.State != ConnectionState.Open)
                    {
                        LogProgress(progressStatus, $"Teardown: Using connectionString [{obfConnectionString}], attempting to open connection; ");
                        connection.Open();
                        LogProgress(progressStatus, $"Teardown: Using connectionString [{obfConnectionString}], connection opened; ");

                    }
                    LogProgress(progressStatus, "Teardown: DELETING all items in [OrganisationStandardDeliveryArea]; ");
                    connection.Execute("DELETE FROM [OrganisationStandardDeliveryArea]");
                    LogProgress(progressStatus, "Teardown: DELETING all items in [OrganisationStandard]; ");
                    connection.Execute("DELETE FROM [OrganisationStandard]");
                }
            }
            catch (Exception e)
            {
                progressStatus.Append("Teardown: DELETION Error; ");
                _logger.LogError($"Progress status: {progressStatus}, Error Message: [{e.Message}]",e);

                throw;
            }

            progressStatus.Append("Teardown: Complete; ");
            _logger.LogInformation($"Progress status: {progressStatus}");

            return progressStatus.ToString();
        }

        public void WriteDeliveryAreas(List<DeliveryArea> deliveryAreas)
        { 
            var connectionString = LocalConnectionString();
            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var deliveryAreasToInsert = new List<DeliveryArea>();

                var currentNumber = connection
                    .ExecuteScalar(
                        "select count(0) from [DeliveryArea]").ToString();

                if (currentNumber == "0")
                {
                    foreach (var deliveryArea in deliveryAreas)
                    {
                        var delArea = deliveryArea;
                        deliveryAreasToInsert.Add(delArea);
                    }

                    if (deliveryAreasToInsert.Count > 0)
                        connection.Execute(
                            "set identity_insert [DeliveryArea] ON; INSERT INTO [DeliveryArea] ([id], [Area],[Status]) VALUES (@Id, @Area, @Status); set identity_insert[DeliveryArea] OFF; ",
                            deliveryAreasToInsert);
                }

                connection.Close();
            }
        }

        public void WriteOrganisationTypes(List<TypeOfOrganisation> organisationTypes)
        {
            var connectionString = LocalConnectionString();
            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var organisationTypesToInsert = new List<TypeOfOrganisation>();

                var currentNumber = connection
                    .ExecuteScalar(
                        "select count(0) from [OrganisationType]").ToString();

                if (currentNumber == "0")
                {
                    foreach (var organisationType in organisationTypes)
                    {
                        var orgType = organisationType;
                        organisationTypesToInsert.Add(orgType);
                    }

                    if (organisationTypesToInsert.Count > 0)
                        connection.Execute(
                            "set identity_insert [OrganisationType] ON; INSERT INTO [OrganisationType] (Id, [Type], [Status], [TypeDescription]) VALUES (@Id, @Type, @Status, @TypeDescription); set identity_insert [OrganisationType] OFF; ",
                            organisationTypesToInsert);
                }
                connection.Close();
            }
        }

        public void WriteOrganisations(List<EpaOrganisation> organisations)
        {
            var connectionString = LocalConnectionString();
            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var organisationsToInsert = new List<EpaOrganisation>();
                var organisationsToUpdate = new List<EpaOrganisation>();

                foreach (var organisation in organisations)
                {
                    var currentCount = connection
                        .ExecuteScalar(
                            "select count(0) from [Organisations] where EndPointAssessorOrganisationId = @EndPointAssessorOrganisationId", organisation)
                            .ToString();

                    var organisationData = new OrganisationData
                    {
                        WebsiteLink = MakeStringSuitableForJson(organisation.OrganisationData?.WebsiteLink),
                        LegalName = MakeStringSuitableForJson(organisation.OrganisationData?.LegalName),
                        Address1 = MakeStringSuitableForJson(organisation.OrganisationData?.Address1),
                        Address2 = MakeStringSuitableForJson(organisation.OrganisationData?.Address2),
                        Address3 = MakeStringSuitableForJson(organisation.OrganisationData?.Address3),
                        Address4 = MakeStringSuitableForJson(organisation.OrganisationData?.Address4),
                        Postcode = MakeStringSuitableForJson(organisation.OrganisationData?.Postcode),
                        CompaniesHouseNumber = MakeStringSuitableForJson(organisation.OrganisationData?.CompaniesHouseNumber),
                        CharitiesCommisionNumber = MakeStringSuitableForJson(organisation.OrganisationData?.CharitiesCommisionNumber)
                    };

                    organisation.OrganisationData = organisationData;

                    if (currentCount == "0")
                    {                 
                        organisationsToInsert.Add(organisation);
                    }
                    else
                    {
                        organisationsToUpdate.Add(organisation);
                    }
                }
              
                var sql = new StringBuilder();

                foreach (var org in organisationsToInsert)
                {
                    var id = ConvertStringToSqlValueString(org.Id.ToString());
                    var organisationData = JsonConvert.SerializeObject(org.OrganisationData);
                    var ukprn = ConvertIntToSqlValueString(org.EndPointAssessorUkprn);
                    var endPointAssessorName = ConvertStringToSqlValueString(org.EndPointAssessorName);

                    var sqlToAppend =
                        "INSERT INTO [Organisations] ([Id],[CreatedAt],[DeletedAt],[EndPointAssessorName],[EndPointAssessorOrganisationId], " +
                        "[EndPointAssessorUkprn],[Status],[UpdatedAt],[OrganisationTypeId],[PrimaryContact],[OrganisationData]) VALUES (" +
                        $@" {id}, getutcdate(), null, {endPointAssessorName}, '{org.EndPointAssessorOrganisationId}'," +
                        $@"{ukprn}, '{org.Status}', null,  {org.OrganisationTypeId}, null, '{organisationData}' ); ";
                    sql.Append(sqlToAppend);
                }

                foreach (var org in organisationsToUpdate)
                {      
                    var organisationData = JsonConvert.SerializeObject(org.OrganisationData);
                
                    var sqlToAppendWhereStatusIsNotDeleted =
                        $@"UPDATE [Organisations] SET [OrganisationTypeId] = {org.OrganisationTypeId}," +
                        $@"[OrganisationData] = '{organisationData}', Status = 'Live' "+
                        $@"WHERE EndPointAssessorOrganisationId = '{org.EndPointAssessorOrganisationId}' and Status !='Deleted'; ";

                    var sqlToAppendWhereStatusIsDeleted =
                        $@"UPDATE [Organisations] SET [OrganisationTypeId] = {org.OrganisationTypeId}," +
                        $@"[OrganisationData] = '{organisationData}', PrimaryContact = '{org.PrimaryContact}' " +
                        $@"WHERE EndPointAssessorOrganisationId = '{org.EndPointAssessorOrganisationId}' and Status = 'Deleted'; ";

                    sql.Append(sqlToAppendWhereStatusIsNotDeleted);
                    sql.Append(sqlToAppendWhereStatusIsDeleted);
                }
                connection.Execute(sql.ToString());                           
                connection.Close();
            }
        }

        public List<EpaOrganisationStandard> WriteEpaOrganisationStandards(List<EpaOrganisationStandard> orgStandards)
        {
            var connectionString = LocalConnectionString();
            var organisationStandardsFromDatabase = new List<EpaOrganisationStandard>();

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                var sql = new StringBuilder();

                var currentNumber = connection.ExecuteScalar("select count(0) from [OrganisationStandard]").ToString();
                if (currentNumber != "0") return organisationStandardsFromDatabase.ToList();
                foreach (var organisationStandard in orgStandards)
                {
                    
                    var comments = ConvertStringToSqlValueString(organisationStandard.Comments);
                    var contactId = ConvertGuidToSqlValueString(organisationStandard.ContactId);
                    var effectiveFrom = ConvertDateToSqlValueString(organisationStandard.EffectiveFrom);
                    var effectiveTo = ConvertDateToSqlValueString(organisationStandard.EffectiveTo);
                    var organisationStandardData = JsonConvert.SerializeObject(organisationStandard.OrganisationStandardData);
                    var dateStandardApprovedOnRegister =
                        ConvertDateToSqlValueString(organisationStandard.DateStandardApprovedOnRegister);

                    var sqlToInsert = "INSERT INTO [OrganisationStandard] ([EndPointAssessorOrganisationId],[StandardCode],[EffectiveFrom],[EffectiveTo],[DateStandardApprovedOnRegister],[Comments],[Status], [ContactId], [OrganisationStandardData])" +
                                      $"VALUES ('{organisationStandard.EndPointAssessorOrganisationId}' ,'{organisationStandard.StandardCode}' ,{effectiveFrom} ,{effectiveTo} ,{dateStandardApprovedOnRegister} ,{comments} ,'{organisationStandard.Status}', {contactId}, '{organisationStandardData}'); ";
                    
                    sql.Append(sqlToInsert);
                }
                connection.Execute(sql.ToString());
                organisationStandardsFromDatabase = connection.QueryAsync<EpaOrganisationStandard>($"select [Id],[EndPointAssessorOrganisationId]," +
                                                                                                    $"[StandardCode],[EffectiveFrom] ,[EffectiveTo]," +
                                                                                                    $"[DateStandardApprovedOnRegister], " +
                                                                                                    $"[Comments],[Status],[ContactId] " +
                                                                                                    $" from [OrganisationStandard]").Result.ToList();

                   connection.Close();
            }

            return organisationStandardsFromDatabase.ToList();
        }

        public void WriteStandardDeliveryAreas(
            List<EpaOrganisationStandardDeliveryArea> organisationStandardDeliveryAreas,
            List<EpaOrganisationStandard> organisationStandards)
        {
            var connectionString = LocalConnectionString();
            var sql = new StringBuilder();

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var currentNumber = connection.ExecuteScalar("select count(0) from [OrganisationStandardDeliveryArea]")
                    .ToString();
                if (currentNumber == "0")
                {
                    foreach (var orgStandardDeliveryArea in organisationStandardDeliveryAreas)
                    {

                        var orgStandard = organisationStandards.FirstOrDefault(
                            x => x.EndPointAssessorOrganisationId ==
                                 orgStandardDeliveryArea.EndPointAssessorOrganisationId &&
                                 x.StandardCode == orgStandardDeliveryArea.StandardCode);

                        if (orgStandard != null)
                        {
                            orgStandardDeliveryArea.OrganisationStandardId = orgStandard.Id;
                        }
                    }
                }

                var orgStandardDeliveryAreasToProcess =
                    organisationStandardDeliveryAreas.Where(x => x.OrganisationStandardId != 0);
              
                foreach (var organisationStandardDeliveryArea in orgStandardDeliveryAreasToProcess)
                {
                    sql.Append($@"INSERT INTO [OrganisationStandardDeliveryArea]
                                        ([OrganisationStandardId]
                                        ,[DeliveryAreaId]
                                        ,[Status])
                                    VALUES
                                        ('{organisationStandardDeliveryArea.OrganisationStandardId}'
                                        , {organisationStandardDeliveryArea.DeliveryAreaId}
                                        , '{organisationStandardDeliveryArea.Status}'); ");

                }
                connection.Execute(sql.ToString());
                connection.Close();
            }
        }

        public List<OrganisationContact>  UpsertThenGatherOrganisationContacts(List<OrganisationContact> contacts)
        {
            var connectionString = LocalConnectionString();

            var contactsFromDatabase = new List<OrganisationContact>();

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var contactsToInsert = new List<OrganisationContact>();
                var contactsToUpdate = new List<OrganisationContact>();

                foreach (var contact in contacts)
                {
                    var numberOfMatches = connection
                        .ExecuteScalar(
                            "select count(0) from [Contacts] where ([EndPointAssessorOrganisationId] = @EndPointAssessorOrganisationId and email = @email) or username=@username ",
                            contact).ToString();

                    if (numberOfMatches == "0")
                    {
                       contactsToInsert.Add(contact);              
                    }
                    else
                    {
                        var username = connection
                                .ExecuteScalar("select username from [Contacts] where ([EndPointAssessorOrganisationId] = @EndPointAssessorOrganisationId and email = @email) or username=@username",
                                    contact).ToString();
                        contact.Username = username;
                        contactsToUpdate.Add(contact);
                    }
                }
               
                var sql = new StringBuilder();

                foreach (var contact in contactsToInsert)
                {
                    var displayName = ConvertStringToSqlValueString(contact.DisplayName);
                    var email = ConvertStringToSqlValueString(contact.Email);
                    var endPointAssessorOrganisationId =
                        ConvertStringToSqlValueString(contact.EndPointAssessorOrganisationId);
                    var userName = ConvertStringToSqlValueString(contact.Username);
                    var phoneNumber = ConvertStringToSqlValueString(contact.PhoneNumber);

                    var detailsToInsert =
                        "INSERT INTO [Contacts] ([Id] ,[CreatedAt] ,[DeletedAt] ,[DisplayName] ,[Email] ,[EndPointAssessorOrganisationId] ,[OrganisationId],  " +
                        "[Status], [UpdatedAt], [Username] ,[PhoneNumber]) VALUES " +
                        $@"(newid(), getutcdate(), null, {displayName}, {email}, {endPointAssessorOrganisationId}, (select  id from organisations where EndPointAssessorOrganisationId = {endPointAssessorOrganisationId}), " +
                        $@"'{contact.Status}', getutcdate(), {userName}, {phoneNumber}); ";

                    sql.Append(detailsToInsert);
                }

                foreach (var contact in contactsToUpdate)
                {
                    var username = ConvertStringToSqlValueString(contact.Username);
                    var phoneNumber = ConvertStringToSqlValueString(contact.PhoneNumber);

                   var detailsToUpdate = 
                        $@"UPDATE [Contacts] Set [PhoneNumber] = {phoneNumber} WHERE "+
                        $@"[username] = {username}; ";

                    sql.Append(detailsToUpdate);
                }

                connection.Execute(sql.ToString());

                contactsFromDatabase = connection.QueryAsync<OrganisationContact>("select * from [Contacts]").Result.ToList();
                connection.Close();
            }

            return contactsFromDatabase;
        }
        private string LocalConnectionString()
        {
            var connectionString = _configuration.SqlConnectionString;
            connectionString = connectionString.Replace("Pooling=False", "Pooling=True");
            return connectionString;
        }
        private void LogProgress(StringBuilder progressStatus, string status)
        {
            progressStatus.Append(status);
            _logger.LogInformation(status);
        }

        private static string ConvertStringToSqlValueString(string stringToProcess)
        {
            return stringToProcess == null
                ? "null"
                : $@"'{stringToProcess.Replace("'","''")}'";
        }

        private static string ConvertGuidToSqlValueString(Guid? stringToProcess)
        {
            return stringToProcess == null
                ? "null"
                : $@"'{stringToProcess}'";
        }

        private static string MakeStringSuitableForJson(string stringToProcess)
        {
            return stringToProcess == null
                ? "null"
                : $@"{stringToProcess.Replace("'", "''")}";
        }

        private static string ConvertIntToSqlValueString(int? intToProcess)
        {
            return !intToProcess.HasValue
                ? "null"
                : $@"{intToProcess}";
        }

        private static string  ConvertDateToSqlValueString (DateTime? dateToProcess)
        {           
            if (!dateToProcess.HasValue)
                return  "null";

            return dateToProcess.Value < new DateTime(1980,1,1) 
                ? "null" 
                : $"'{dateToProcess.Value:yyyy-MM-dd}'";
        }   
    }
}
