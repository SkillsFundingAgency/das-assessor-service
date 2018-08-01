using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public static class OrganisationHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

      
        public static void InsertRecord(OrganisationModel organisation)
        {
            var id = SqlStringService.ConvertStringToSqlValueString(organisation.Id.ToString());
            var createdAt = SqlStringService.ConvertDateToSqlValueString(organisation.CreatedAt);
            var deletedAt = SqlStringService.ConvertDateToSqlValueString(organisation.DeletedAt);
            var assessorName = SqlStringService.ConvertStringToSqlValueString(organisation.EndPointAssessorName);
            var organisationId = SqlStringService.ConvertStringToSqlValueString(organisation.EndPointAssessorOrganisationId);
            var ukprn = SqlStringService.ConvertIntToSqlValueString(organisation.EndPointAssessorUkprn);
            var primaryContact = SqlStringService.ConvertStringToSqlValueString(organisation.PrimaryContact);
            var status = SqlStringService.ConvertStringToSqlValueString(organisation.Status);
            var updatedAt = SqlStringService.ConvertDateToSqlValueString(organisation.UpdatedAt);
            var organisationTypeId = SqlStringService.ConvertIntToSqlValueString(organisation.OrganisationTypeId);
            var organisationData = SqlStringService.ConvertStringToSqlValueString(organisation.OrganisationData);
            var sql =
                "INSERT INTO [Organisations] ([Id] ,[CreatedAt],[DeletedAt],[EndPointAssessorName],[EndPointAssessorOrganisationId],  " +
                "[EndPointAssessorUkprn],[PrimaryContact],[Status],[UpdatedAt],[OrganisationTypeId],[OrganisationData]) VALUES " +
                $@"({id},{createdAt}, {deletedAt}, {assessorName}, {organisationId}, {ukprn}, {primaryContact}, {status}, {updatedAt}, {organisationTypeId}, {organisationData}); ";

            DatabaseService.Execute(sql);
        }

        public static void DeleteRecord(Guid id)
        {
            var idToDelete = SqlStringService.ConvertStringToSqlValueString(id.ToString());
            var sql = $@"DELETE from Organisations where id = {idToDelete}";
            DatabaseService.Execute(sql);
        }
    }
}

