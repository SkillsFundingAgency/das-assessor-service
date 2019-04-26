using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;
using System.Linq;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;

namespace SFA.DAS.AssessorService.Data
{
    public class RegisterRepository : IRegisterRepository
    {

        private readonly IWebConfiguration _configuration;

        public RegisterRepository(IWebConfiguration configuration)
        {
            _configuration = configuration;
            SqlMapper.AddTypeHandler(typeof(OrganisationData), new OrganisationDataHandler());
            SqlMapper.AddTypeHandler(typeof(OrganisationStandardData), new OrganisationStandardDataHandler());
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
                    $@"VALUES (@id, GetUtcDate(), @name, @organisationId, @ukprn, 'New', @organisationTypeId,  @orgData)",
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
                    "UPDATE [Organisations] SET [UpdatedAt] = GetUtcDate(), [EndPointAssessorName] = @Name, " +
                    "[EndPointAssessorUkprn] = @ukprn, [OrganisationTypeId] = @organisationTypeId, " +
                    "[OrganisationData] = @orgData, Status = @status WHERE [EndPointAssessorOrganisationId] = @organisationId",
                    new {org.Name, org.Ukprn, org.OrganisationTypeId, orgData, org.Status, org.OrganisationId});
       
                return org.OrganisationId;
            }
        }

        public async Task<string>CreateEpaOrganisationStandard(EpaOrganisationStandard organisationStandard, List<int> deliveryAreas)
        {
           
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();


                var osdaId = connection.Query<string>(
                    "INSERT INTO [dbo].[OrganisationStandard] ([EndPointAssessorOrganisationId],[StandardCode],[EffectiveFrom],[EffectiveTo],[DateStandardApprovedOnRegister] ,[Comments],[Status], [ContactId], [OrganisationStandardData]) VALUES (" +
                    "@organisationId, @standardCode, @effectiveFrom, @effectiveTo, getutcdate(), @comments, 'Live', @ContactId,  @OrganisationStandardData); SELECT CAST(SCOPE_IDENTITY() as varchar); ",
                    new
                    {
                        organisationStandard.OrganisationId, organisationStandard.StandardCode,
                        organisationStandard.EffectiveFrom, organisationStandard.EffectiveTo,
                        organisationStandard.DateStandardApprovedOnRegister, organisationStandard.Comments,
                        organisationStandard.ContactId, organisationStandard.OrganisationStandardData
                    }).Single();

                foreach (var deliveryAreaId in deliveryAreas.Distinct())
                {
                    connection.Execute("INSERT INTO OrganisationStandardDeliveryArea ([OrganisationStandardId],DeliveryAreaId, Status) VALUES " + 
                                        "(@osdaId, @deliveryAreaId,'Live'); ",
                                    new { osdaId, deliveryAreaId}
                                    );
                }                      
                        
                return osdaId;
            }
            
        }

        public async Task<string> UpdateEpaOrganisationStandard(EpaOrganisationStandard orgStandard,
            List<int> deliveryAreas)
        {

            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var osdaId = connection.Query<string>(
                    "UPDATE [OrganisationStandard] SET [EffectiveFrom] = @effectiveFrom, [EffectiveTo] = @EffectiveTo, " +
                    "[Comments] = @comments, [ContactId] = @contactId, [OrganisationStandardData] = @organisationStandardData, [Status]='Live' " +
                    "WHERE [EndPointAssessorOrganisationId] = @organisationId and [StandardCode] = @standardCode; SELECT top 1 id from [organisationStandard] where  [EndPointAssessorOrganisationId] = @organisationId and [StandardCode] = @standardCode;",
                    new
                    {
                        orgStandard.EffectiveFrom,
                        orgStandard.EffectiveTo,
                        orgStandard.Comments,
                        orgStandard.ContactId,
                        orgStandard.OrganisationId,
                        orgStandard.StandardCode,
                        orgStandard.OrganisationStandardData
                    }).Single();

                connection.Execute(
                    "Delete from OrganisationStandardDeliveryArea where OrganisationStandardId = @osdaId and DeliveryAreaId not in @deliveryAreas",
                    new {osdaId, deliveryAreas});

                foreach (var deliveryAreaId in deliveryAreas.Distinct())
                {
                    connection.Execute(
                        "IF NOT EXISTS (select * from OrganisationStandardDeliveryArea where OrganisationStandardId = @osdaId and DeliveryAreaId = @DeliveryAreaId) " +
                        "INSERT INTO OrganisationStandardDeliveryArea ([OrganisationStandardId],DeliveryAreaId, Status) VALUES " +
                        "(@osdaId, @deliveryAreaId,'Live'); ",
                        new {osdaId, deliveryAreaId}
                    );
                }

                    connection.Execute(
                        "UPDATE [OrganisationStandard] SET [DateStandardApprovedOnRegister] = getutcdate() where Id = @osdaId and [DateStandardApprovedOnRegister] is null",
                        new { osdaId }
                    );
              
                return osdaId;
            }
        }

        public async Task<string> CreateEpaOrganisationContact(EpaContact contact)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();


                connection.Execute(
                    $@"INSERT INTO [dbo].[Contacts] ([Id],[CreatedAt],[DisplayName],[Email],[EndPointAssessorOrganisationId],[OrganisationId],[Status],[Username],[PhoneNumber], [GivenNames], [FamilyName], [SigninId], [SigninType]) " +
                    $@"VALUES (@id,getutcdate(), @displayName, @email, @endPointAssessorOrganisationId," +
                    $@"(select id from organisations where EndPointAssessorOrganisationId=@endPointAssessorOrganisationId), " +
                    $@"'Live', @username, @PhoneNumber, @FirstName, @LastName, @SigninId, @SigninType);",
                    new
                    {
                        contact.Id,
                        contact.DisplayName,
                        contact.Email,
                        contact.EndPointAssessorOrganisationId,
                        contact.Username,
                        contact.PhoneNumber,
                        contact.FirstName,
                        contact.LastName,
                        contact.SigninId,
                        contact.SigninType
                    });

                connection.Execute("UPDATE [dbo].[Organisations] set PrimaryContact=@username WHERE EndPointAssessorOrganisationId = @endPointAssessorOrganisationId and PrimaryContact is null",
                    new
                    {
                        contact.EndPointAssessorOrganisationId, contact.Username
                    });
                return contact.Id.ToString();
            }
        }

        public async Task<string> AssociateDefaultRoleWithContact(EpaContact contact)
        {
            using(var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                
                connection.Execute(
                    @"INSERT INTO[ContactRoles] SELECT ab1.*, co1.id contactid FROM( SELECT newid() Id, 'SuperUser' Rolename) ab1 CROSS JOIN[Contacts] co1 WHERE co1.[Status] = 'Live'" +
                    @" AND EXISTS(SELECT NULL FROM Organisations og1 WHERE og1.id = co1.OrganisationId AND og1.[Status] != 'Deleted')" +
                    @" AND NOT EXISTS(SELECT NULL FROM[ContactRoles] co2 WHERE co2.ContactId = @Id)" +
                    @" AND co1.Id = @Id",
                    new
                    {
                        contact.Id,
                    });

                
                return contact.Id.ToString();
            }
        }

        public async Task<string> AssociateAllPrivilegesWithContact(EpaContact contact)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                connection.Execute(
                    @" insert into[ContactsPrivileges]" +
                    @" select co1.id, pr1.id from Contacts co1 cross join[Privileges] pr1" +
                    @"  where co1.status = 'Live'  and co1.username not like 'unknown%' and co1.username != 'manual' and co1.Id = @Id",
                    new
                    {
                        contact.Id,
                    });


                return contact.Id.ToString();
            }
        }

        public async Task<string> UpdateEpaOrganisationContact(EpaContact contact, string actionChoice)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();


                connection.Execute(
                    "UPDATE [Contacts] SET [DisplayName] = @displayName, [Email] = @email, " +
                    "[GivenNames] = @firstName, [FamilyName] = @lastName, " +
                    "[PhoneNumber] = @phoneNumber, [updatedAt] = getUtcDate() " +
                    "WHERE [Id] = @Id ",
                    new { contact.DisplayName, contact.Email, contact.FirstName, contact.LastName, contact.PhoneNumber, contact.Id});

                if (actionChoice == "MakePrimaryContact")
                    connection.Execute("update o set PrimaryContact = c.Username from organisations o " +
                                       "inner join contacts c on o.EndPointAssessorOrganisationId = c.EndPointAssessorOrganisationId " +
                                       "Where c.id = @Id",
                        new {contact.Id});

                return contact.Id.ToString();
            }
        }

        public async Task<string> AssociateOrganisationWithContact(Guid contactId, EpaOrganisation org, string status, string actionChoice)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();


                connection.Execute(
                    "UPDATE [Contacts] SET [EndPointAssessorOrganisationId] = @OrganisationId, [OrganisationId] = @Id, " +
                    "[Status] = @status, [updatedAt] = getUtcDate() " +
                    "WHERE [Id] = @contactId ",
                    new { org.OrganisationId, org.Id, contactId,  status });

                if (actionChoice == "MakePrimaryContact")
                    connection.Execute("update o set PrimaryContact = c.Username from organisations o " +
                                       "inner join contacts c on o.EndPointAssessorOrganisationId = c.EndPointAssessorOrganisationId " +
                                       "Where c.id = @contactId",
                        new { contactId });

                return contactId.ToString();
            }
        }
    }
}
