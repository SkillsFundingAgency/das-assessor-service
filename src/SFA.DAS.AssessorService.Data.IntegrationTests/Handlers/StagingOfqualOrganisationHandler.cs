using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public class StagingOfqualOrganisationHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(StagingOfqualOrganisationModel stagingOfqualOrganisation)
        {
            var sqlToInsert =
                "INSERT INTO [dbo].[StagingOfqualOrganisation]" +
                    "([RecognitionNumber]" +
                    ", [Name]" +
                    ", [LegalName]" +
                    ", [Acronym]" +
                    ", [Email]" +
                    ", [Website]" +
                    ", [HeadOfficeAddressLine1]" +
                    ", [HeadOfficeAddressLine2]" +
                    ", [HeadOfficeAddressTown]" +
                    ", [HeadOfficeAddressCounty]" +
                    ", [HeadOfficeAddressPostcode]" +
                    ", [HeadOfficeAddressCountry]" +
                    ", [HeadOfficeAddressTelephone]" +
                    ", [OfqualStatus]" +
                    ", [OfqualRecognisedFrom]" +
                    ", [OfqualRecognisedTo]) " +
                "VALUES " +
                    "(@recognitionNumber" +
                    ", @name" +
                    ", @legalName" +
                    ", @acronym" +
                    ", @email" +
                    ", @website" +
                    ", @headOfficeAddressLine1" +
                    ", @headOfficeAddressLine2" +
                    ", @headOfficeAddressTown" +
                    ", @headOfficeAddressCounty" +
                    ", @headOfficeAddressPostcode" +
                    ", @headOfficeAddressCountry" +
                    ", @headOfficeAddressTelephone" +
                    ", @ofqualStatus" +
                    ", @ofqualRecognisedFrom" +
                    ", @ofqualRecognisedTo)";

            DatabaseService.Execute(sqlToInsert, stagingOfqualOrganisation);
        }

        public static void InsertRecords(List<StagingOfqualOrganisationModel> stagingOfqualOrganisations)
        {
            foreach (var stagingOfqualOrganisation in stagingOfqualOrganisations)
            {
                InsertRecord(stagingOfqualOrganisation);
            }
        }

        public static void DeleteAllRecords()
        {
            var sql = "DELETE FROM [StagingOfqualOrganisation]";

            DatabaseService.Execute(sql);
        }
    }
}
