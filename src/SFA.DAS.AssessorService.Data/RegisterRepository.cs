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
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;

namespace SFA.DAS.AssessorService.Data
{
    public class RegisterRepository : Repository, IRegisterRepository
    {
        private readonly ILogger<RegisterRepository> _logger;

        public RegisterRepository(IUnitOfWork unitOfWork, ILogger<RegisterRepository> logger)
            : base(unitOfWork)
        {
            _logger = logger;
            
            SqlMapper.AddTypeHandler(typeof(OrganisationData), new OrganisationDataHandler());
            SqlMapper.AddTypeHandler(typeof(OrganisationStandardData), new OrganisationStandardDataHandler());
        }

        public async Task<string> CreateEpaOrganisation(EpaOrganisation org)
        {
            var orgData = JsonConvert.SerializeObject(org.OrganisationData);
            await _unitOfWork.Connection.ExecuteAsync(
                "INSERT INTO [Organisations] ([Id],[CreatedAt],[EndPointAssessorName],[EndPointAssessorOrganisationId], " +
                    "[EndPointAssessorUkprn],[Status],[OrganisationTypeId],[OrganisationData]) " +
                    $@"VALUES (@id, GetUtcDate(), @name, @organisationId, @ukprn, @status, @organisationTypeId,  @orgData)",
                    new {org.Id, org.Name, org.OrganisationId, org.Ukprn, org.Status, org.OrganisationTypeId, orgData}
                );

            return org.OrganisationId;
        }

        public async Task<string> UpdateEpaOrganisation(EpaOrganisation org)
        {
            var orgData = JsonConvert.SerializeObject(org.OrganisationData);
            await _unitOfWork.Connection.ExecuteAsync(
                "UPDATE [Organisations] SET [UpdatedAt] = GetUtcDate(), [EndPointAssessorName] = @Name, " +
                    "[EndPointAssessorUkprn] = @ukprn, [OrganisationTypeId] = @organisationTypeId, " +
                    "[OrganisationData] = @orgData, Status = @status WHERE [EndPointAssessorOrganisationId] = @organisationId",
                    new { org.Name, org.Ukprn, org.OrganisationTypeId, orgData, org.Status, org.OrganisationId});

            _logger.LogInformation($"Updated EPAO Organisation {org.OrganisationId} with status = {org.Status}");

            return org.OrganisationId;
        }

        public async Task<string> CreateEpaOrganisationStandard(EpaOrganisationStandard organisationStandard, List<int> deliveryAreas)
        {
            var sqlToSelectExisting =
                "select Id FROM [OrganisationStandard] " +
                    "WHERE EndPointAssessorOrganisationId = @organisationId and standardCode = @standardCode";
            var orgStandardId = await _unitOfWork.Connection.ExecuteScalarAsync<int>(sqlToSelectExisting, new { organisationId = organisationStandard.OrganisationId, standardCode = organisationStandard.StandardCode });

            if (default(int) == orgStandardId)
            {
                orgStandardId = (await _unitOfWork.Connection.QueryAsync<int>(
                    "INSERT INTO [dbo].[OrganisationStandard] ([EndPointAssessorOrganisationId],[StandardCode],[EffectiveFrom],[EffectiveTo],[DateStandardApprovedOnRegister] ,[Comments],[Status], [ContactId], [OrganisationStandardData], StandardReference) VALUES (" +
                        "@organisationId, @standardCode, @effectiveFrom, @effectiveTo, getutcdate(), @comments, 'Live', @ContactId,  @OrganisationStandardData, @StandardReference); SELECT CAST(SCOPE_IDENTITY() as varchar); ",
                        new
                        {
                            organisationStandard.OrganisationId,
                            organisationStandard.StandardCode,
                            organisationStandard.EffectiveFrom,
                            organisationStandard.EffectiveTo,
                            organisationStandard.DateStandardApprovedOnRegister,
                            organisationStandard.Comments,
                            organisationStandard.ContactId,
                            organisationStandard.OrganisationStandardData,
                            organisationStandard.StandardReference
                        })).Single();

                foreach (var deliveryAreaId in deliveryAreas.Distinct())
                {
                    await _unitOfWork.Connection.ExecuteAsync(
                        "INSERT INTO OrganisationStandardDeliveryArea ([OrganisationStandardId],DeliveryAreaId, Status) VALUES " +
                        "(@osdaId, @deliveryAreaId,'Live'); ",
                        new { osdaId = orgStandardId, deliveryAreaId });
                }
            }
            else
            {
                // Fix StandardReference on the existing record
                await _unitOfWork.Connection.ExecuteAsync(
                    "UPDATE [dbo].[OrganisationStandard] SET StandardReference = @standardReference WHERE Id = @id",
                    new
                    {
                        standardReference = organisationStandard.StandardReference,
                        id = orgStandardId
                    });
            }

            if (null != organisationStandard.StandardVersions)
            {
                foreach (var version in organisationStandard.StandardVersions)
                {
                    var standardUid = $"{organisationStandard.StandardReference.Trim()}_{version.Trim()}";

                    await _unitOfWork.Connection.ExecuteAsync(
                        "INSERT INTO OrganisationStandardVersion (StandardUid, Version, OrganisationStandardId, EffectiveFrom, EffectiveTo, DateVersionApproved, Comments, Status) " +
                            "VALUES(@StandardUid, @Version, @OrganisationStandardId, @EffectiveFrom, @EffectiveTo, @DateVersionApproved, @Comments, 'Live')",
                            new
                            {
                                standardUid,
                                version,
                                OrganisationStandardId = orgStandardId,
                                organisationStandard.EffectiveFrom,
                                organisationStandard.EffectiveTo,
                                DateVersionApproved = organisationStandard.DateStandardApprovedOnRegister,
                                organisationStandard.Comments
                            });
                }
            }

            return orgStandardId.ToString();
        }

        public async Task<string> UpdateEpaOrganisationStandardAndOrganisationStandardVersions(EpaOrganisationStandard orgStandard,
            List<int> deliveryAreas)
        {
            var osdaId = (await _unitOfWork.Connection.QueryAsync<string>(
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
                    })).Single();

            await _unitOfWork.Connection.ExecuteAsync(
                "Delete from OrganisationStandardDeliveryArea where OrganisationStandardId = @osdaId and DeliveryAreaId not in @deliveryAreas",
                new {osdaId, deliveryAreas});

            foreach (var deliveryAreaId in deliveryAreas.Distinct())
            {
                await _unitOfWork.Connection.ExecuteAsync(
                    "IF NOT EXISTS (select * from OrganisationStandardDeliveryArea where OrganisationStandardId = @osdaId and DeliveryAreaId = @DeliveryAreaId) " +
                        "INSERT INTO OrganisationStandardDeliveryArea ([OrganisationStandardId],DeliveryAreaId, Status) VALUES " +
                        "(@osdaId, @deliveryAreaId,'Live'); ",
                        new {osdaId, deliveryAreaId});
            }

            await _unitOfWork.Connection.ExecuteAsync(
                "UPDATE [OrganisationStandard] SET [DateStandardApprovedOnRegister] = getutcdate() where Id = @osdaId and [DateStandardApprovedOnRegister] is null",
                    new { osdaId });


            if (null != orgStandard.StandardVersions)
            {
                foreach (var version in orgStandard.StandardVersions)
                {
                    var standardUid = $"{orgStandard.StandardReference.Trim()}_{version.Trim()}";

                    await _unitOfWork.Connection.ExecuteAsync(
                        "IF NOT EXISTS (select * from OrganisationStandardVersion where StandardUId = @StandardUid and Version = @version and OrganisationStandardId = @OrganisationStandardId) " +
                        "INSERT INTO OrganisationStandardVersion (StandardUid, Version, OrganisationStandardId, EffectiveFrom, EffectiveTo, DateVersionApproved, Comments, Status) " +
                            "VALUES(@StandardUid, @Version, @OrganisationStandardId, @EffectiveFrom, @EffectiveTo, @DateVersionApproved, @Comments, 'Live')",
                            new
                            {
                                standardUid,
                                version,
                                OrganisationStandardId = osdaId,
                                orgStandard.EffectiveFrom,
                                orgStandard.EffectiveTo,
                                DateVersionApproved = orgStandard.DateStandardApprovedOnRegister,
                                orgStandard.Comments
                            });

                    await _unitOfWork.Connection.ExecuteAsync(
                        @"UPDATE [OrganisationStandardVersion] 
                                        SET [EffectiveFrom] = @effectiveFrom,
                                            [EffectiveTo] = @effectiveTo
                                        WHERE
                                            [OrganisationStandardId] = @id",
                        new
                        {
                            orgStandard.EffectiveFrom,
                            orgStandard.EffectiveTo,
                            orgStandard.Id
                        });
                }
            }

            return osdaId;
        }

        public async Task<string> CreateEpaOrganisationContact(EpaContact contact)
        {
            await _unitOfWork.Connection.ExecuteAsync(
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
                        firstName = string.IsNullOrEmpty(contact.FirstName) ? " " : contact.FirstName,
                        lastName = string.IsNullOrEmpty(contact.LastName) ? " " : contact.LastName,
                        contact.SigninId,
                        contact.SigninType
                    });

            await _unitOfWork.Connection.ExecuteAsync(
                "UPDATE [dbo].[Organisations] set PrimaryContact=@username WHERE EndPointAssessorOrganisationId = @endPointAssessorOrganisationId and PrimaryContact is null",
                    new
                    {
                        contact.EndPointAssessorOrganisationId,
                        contact.Username
                    });

            return contact.Id.ToString();
        }

        public async Task<string> AssociateAllPrivilegesWithContact(EpaContact contact)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                @" insert into[ContactsPrivileges]" +
                    @" select co1.id, pr1.id from Contacts co1 cross join[Privileges] pr1" +
                    @"  where co1.status = 'Live'  and co1.username not like 'unknown%' and co1.username != 'manual' and co1.Id = @Id" +
                    @"  AND NOT EXISTS(SELECT NULL FROM [ContactsPrivileges] WHERE ContactId = co1.id AND PrivilegeId = pr1.id)",
                    new
                    {
                        contact.Id,
                    });

            return contact.Id.ToString();
        }

        //Fix for ON-2047
        public async Task<string> AssociateDefaultPrivilegesWithContact(EpaContact contact)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                @" INSERT INTO[ContactsPrivileges](contactid, PrivilegeId)" +
                    @" SELECT contactid, PrivilegeId FROM (select co1.id contactid, pr1.id PrivilegeId from Contacts co1 cross join[Privileges] pr1" +
                    @" where MustBeAtLeastOneUserAssigned = 1 and co1.username not like 'unknown%' and co1.username != 'manual' and co1.Id = @Id" +
                    @" and co1.Status = 'Live') ab1" +
                    @" WHERE NOT EXISTS(SELECT NULL FROM[ContactsPrivileges] WHERE ContactId = ab1.ContactId AND PrivilegeId = ab1.PrivilegeId)",
                    new
                    {
                        contact.Id,
                    });

            return contact.Id.ToString();
        }

        public async Task<string> UpdateEpaOrganisationContact(EpaContact contact, string actionChoice)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                "UPDATE [Contacts] SET [DisplayName] = @displayName, [Email] = @email, " +
                    "[GivenNames] = @firstName, [FamilyName] = @lastName, " +
                    "[PhoneNumber] = @phoneNumber, [updatedAt] = getUtcDate() " +
                    "WHERE [Id] = @Id ",
                    new
                    {
                        contact.DisplayName,
                        contact.Email,
                        firstName = string.IsNullOrEmpty(contact.FirstName) ? " " : contact.FirstName,
                        lastName = string.IsNullOrEmpty(contact.LastName) ? " " : contact.LastName,
                        contact.PhoneNumber,
                        contact.Id
                    });

            if (actionChoice == "MakePrimaryContact")
            {
                await _unitOfWork.Connection.ExecuteAsync(
                    "update o set PrimaryContact = c.Username from organisations o " +
                                       "inner join contacts c on o.EndPointAssessorOrganisationId = c.EndPointAssessorOrganisationId " +
                                       "Where c.id = @Id",
                        new { contact.Id });
            }

            return contact.Id.ToString();
        }

        public async Task<string> AssociateOrganisationWithContact(Guid contactId, EpaOrganisation org, string status, string actionChoice)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                "UPDATE [Contacts] SET [EndPointAssessorOrganisationId] = @OrganisationId, [OrganisationId] = @Id, " +
                    "[Status] = @status, [updatedAt] = getUtcDate() " +
                    "WHERE [Id] = @contactId ",
                    new { org.OrganisationId, org.Id, contactId, status });

            if (actionChoice == "MakePrimaryContact")
            {
                await _unitOfWork.Connection.ExecuteAsync(
                    "update o set PrimaryContact = c.Username from organisations o " +
                                       "inner join contacts c on o.EndPointAssessorOrganisationId = c.EndPointAssessorOrganisationId " +
                                       "Where c.id = @contactId",
                        new { contactId });
            }

            return contactId.ToString();
        }

        public async Task UpdateEpaOrganisationPrimaryContact(Guid contactId, string contactUsername)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                "update o set PrimaryContact = c.Username from organisations o " +
                                       "inner join contacts c on o.EndPointAssessorOrganisationId = c.EndPointAssessorOrganisationId " +
                                       "Where c.id = @contactId And o.PrimaryContact = @contactUsername",
                        new { contactId, contactUsername });

        }
    }
}
