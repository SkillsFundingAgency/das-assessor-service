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
using SFA.DAS.AssessorService.Domain.Entities.AssessmentOrganisations;
using SFA.DAS.AssessorService.Settings;

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
            try
            {
                var connectionString = _configuration.SqlConnectionString;

                progressStatus.Append("Teardown: Opening connection to database; ");
                using (var connection = new SqlConnection(connectionString))
                {
                    if (connection.State != ConnectionState.Open)
                        connection.Open();
                    
                    progressStatus.Append("Teardown: DELETING all items in [OrganisationStandardDeliveryArea]; ");
                    connection.Execute("DELETE FROM [OrganisationStandardDeliveryArea]");
                    progressStatus.Append("Teardown: DELETING all items in [OrganisationStandard]; ");
                    connection.Execute("DELETE FROM [OrganisationStandard]");
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
            var connectionString = _configuration.SqlConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var deliveryAreasToInsert = new List<DeliveryArea>();
 
                foreach (var deliveryArea in deliveryAreas)
                {
                    var currentNumber = connection
                        .ExecuteScalar(
                            "select count(0) from [DeliveryArea] where Id = @Id",  deliveryArea).ToString();
                    if (currentNumber != "0") continue;
                    var delArea = deliveryArea;
                    deliveryAreasToInsert.Add(delArea);
                }

                 if (deliveryAreasToInsert.Count > 0)
                    connection.Execute(
                        "set identity_insert [DeliveryArea] ON; INSERT INTO [DeliveryArea] ([id], [Area],[Status]) VALUES (@Id, @Area, @Status); set identity_insert[DeliveryArea] OFF; ",
                        deliveryAreasToInsert);

                connection.Close();
            }
        }

        public void WriteOrganisationTypes(List<TypeOfOrganisation> organisationTypes)
        {
            var connectionString = _configuration.SqlConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var organisationTypesToInsert = new List<TypeOfOrganisation>();

                foreach (var organisationType in organisationTypes)
                {
                    var currentNumber = connection
                        .ExecuteScalar(
                            "select count(0) from [OrganisationType] where Type = @Type", organisationType).ToString();
                  
                    if (currentNumber != "0") continue;
                    var orgType = organisationType;
                    organisationTypesToInsert.Add(orgType);
                }

                if (organisationTypesToInsert.Count>0)
                    connection.Execute(
                        "set identity_insert [OrganisationType] ON; INSERT INTO [OrganisationType] (Id, [Type], [Status]) VALUES (@Id, @Type, @Status); set identity_insert [OrganisationType] OFF; ",
                        organisationTypesToInsert);
             
               connection.Close();
            }
        }

        public void WriteOrganisations(List<EpaOrganisation> organisations)
        {
            var connectionString = _configuration.SqlConnectionString;
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
                        Postcode = MakeStringSuitableForJson(organisation.OrganisationData?.Postcode)
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
                        "[EndPointAssessorUkprn],[PrimaryContact],[Status],[UpdatedAt],[OrganisationTypeId],[OrganisationData]) VALUES (" +
                        $@" {id}, getutcdate(), null, {endPointAssessorName}, '{org.EndPointAssessorOrganisationId}'," +
                        $@"{ukprn}, null, '{org.Status}', null,  {org.OrganisationTypeId}, '{organisationData}' ); ";
                    sql.Append(sqlToAppend);
                }

                foreach (var org in organisationsToUpdate)
                {      
                    var organisationData = JsonConvert.SerializeObject(org.OrganisationData);
                
                    var sqlToAppend =
                        $@"UPDATE [Organisations] SET [OrganisationTypeId] = {org.OrganisationTypeId}," +
                        $@"[OrganisationData] = '{organisationData}' "+
                        $@"WHERE EndPointAssessorOrganisationId = '{org.EndPointAssessorOrganisationId}'; ";
                    sql.Append(sqlToAppend);
                }
                connection.Execute(sql.ToString());                           
                connection.Close();
            }
        }

        public List<EpaOrganisationStandard> WriteEpaOrganisationStandards(List<EpaOrganisationStandard> orgStandards)
        {
            var connectionString = _configuration.SqlConnectionString;
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
                    var dateStandardApprovedOnRegister =
                        ConvertDateToSqlValueString(organisationStandard.DateStandardApprovedOnRegister);

                    var sqlToInsert = "INSERT INTO [OrganisationStandard] ([EndPointAssessorOrganisationId],[StandardCode],[EffectiveFrom],[EffectiveTo],[DateStandardApprovedOnRegister],[Comments],[Status], [ContactId])" +
                                      $"VALUES ('{organisationStandard.EndPointAssessorOrganisationId}' ,'{organisationStandard.StandardCode}' ,{effectiveFrom} ,{effectiveTo} ,{dateStandardApprovedOnRegister} ,{comments} ,'{organisationStandard.Status}', {contactId}); ";

                    sql.Append(sqlToInsert);
                }
                connection.Execute(sql.ToString());
                organisationStandardsFromDatabase = connection.QueryAsync<EpaOrganisationStandard>("select * from [OrganisationStandard]").Result.ToList();                
                connection.Close();
            }

            return organisationStandardsFromDatabase.ToList();
        }

        public void WriteStandardDeliveryAreas(
            List<EpaOrganisationStandardDeliveryArea> organisationStandardDeliveryAreas,
            List<EpaOrganisationStandard> organisationStandards)
        {
            var connectionString = _configuration.SqlConnectionString;
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
                                        ,[Comments]
                                        ,[Status])
                                    VALUES
                                        ('{organisationStandardDeliveryArea.OrganisationStandardId}'
                                        , {organisationStandardDeliveryArea.DeliveryAreaId}
                                        , '{organisationStandardDeliveryArea.Comments}'
                                        , '{organisationStandardDeliveryArea.Status}'); ");

                }
                connection.Execute(sql.ToString());
                connection.Close();
            }
        }

        public List<OrganisationContact>  UpsertThenGatherOrganisationContacts(List<OrganisationContact> contacts)
        {
            var connectionString = _configuration.SqlConnectionString;

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
                            "select count(0) from [Contacts] where [EndPointAssessorOrganisationId] = @EndPointAssessorOrganisationId and email = @email",
                            contact).ToString();
                    if (numberOfMatches == "0")
                    {
                       contactsToInsert.Add(contact);              
                    }
                    else
                    {
                        var username = connection
                            .ExecuteScalar("select username from [Contacts] where [EndPointAssessorOrganisationId] = @EndPointAssessorOrganisationId and email = @email",
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
