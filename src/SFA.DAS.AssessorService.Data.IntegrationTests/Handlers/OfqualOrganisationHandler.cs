using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public class OfqualOrganisationHandler : HandlerBase
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(OfqualOrganisationModel ofqualOrganisation)
        {
            var sqlToInsert =
                "INSERT INTO [dbo].[OfqualOrganisation]" +
                    "([Id]" +
                    ", [RecognitionNumber]" +
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
                    ", [OfqualRecognisedTo]" +
                    ", [CreatedAt]" +
                    ", [UpdatedAt]) " +
                "VALUES " +
                    "(@id" +
                    ", @recognitionNumber" +
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
                    ", @ofqualRecognisedTo" +
                    ", @createdAt" +
                    ", @updatedAt)";

            DatabaseService.Execute(sqlToInsert, ofqualOrganisation);
        }

        public static void InsertRecords(List<OfqualOrganisationModel> ofqualOrganisations)
        {
            foreach (var ofqualOrganisation in ofqualOrganisations)
            {
                InsertRecord(ofqualOrganisation);
            }
        }

        public static OfqualOrganisationModel Create(Guid? id, string recognitionNumber, string name, string legalName, string acronym, string email, string website, string headOfficeAddressLine1,
            string headOfficeAddressLine2, string headOfficeAddressTown, string headOfficeAddressCounty, string headOfficeAddressPostcode,
            string headOfficeAddressCountry, string headOfficeAddressTelephone, string ofqualStatus, DateTime ofqualRecognisedFrom, DateTime? ofqualRecognisedTo,
            DateTime createdAt, DateTime? updatedAt)
        {
            return new OfqualOrganisationModel
            {
                Id = id,
                RecognitionNumber = recognitionNumber,
                Name = name,
                LegalName = legalName,
                Acronym = acronym,
                Email = email,
                Website = website,
                HeadOfficeAddressLine1 = headOfficeAddressLine1,
                HeadOfficeAddressLine2 = headOfficeAddressLine2,
                HeadOfficeAddressTown = headOfficeAddressTown,
                HeadOfficeAddressCounty = headOfficeAddressCounty,
                HeadOfficeAddressPostcode = headOfficeAddressPostcode,
                HeadOfficeAddressCountry = headOfficeAddressCountry,
                HeadOfficeAddressTelephone = headOfficeAddressTelephone,
                OfqualStatus = ofqualStatus,
                OfqualRecognisedFrom = ofqualRecognisedFrom,
                OfqualRecognisedTo = ofqualRecognisedTo,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };
        }

        public static async Task<OfqualOrganisationModel> QueryFirstOrDefaultAsync(OfqualOrganisationModel ofqualOrganisation)
        {
            var sqlToQuery =
                "SELECT" +
                    "[Id]" +
                    ", [RecognitionNumber]" +
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
                    ", [OfqualRecognisedTo]" +
                    ", [CreatedAt]" +
                    ", [UpdatedAt]" +
                "FROM [OfqualOrganisation] " +
                "WHERE (Id = @id OR @id IS NULL) " + // when @id is null then Id is not predicated
                    "AND RecognitionNumber = @recognitionNumber " +
                    "AND Name = @name " +
                    "AND LegalName = @legalName " +
                   $"AND {NullQueryParam(ofqualOrganisation, p => p.Acronym)} " +
                   $"AND {NullQueryParam(ofqualOrganisation, p => p.Email)} " +
                   $"AND {NullQueryParam(ofqualOrganisation, p => p.Website)} " +
                   $"AND {NullQueryParam(ofqualOrganisation, p => p.HeadOfficeAddressLine1)} " +
                   $"AND {NullQueryParam(ofqualOrganisation, p => p.HeadOfficeAddressLine2)} " +
                   $"AND {NullQueryParam(ofqualOrganisation, p => p.HeadOfficeAddressTown)} " +
                   $"AND {NullQueryParam(ofqualOrganisation, p => p.HeadOfficeAddressCounty)} " +
                   $"AND {NullQueryParam(ofqualOrganisation, p => p.HeadOfficeAddressPostcode)} " +
                   $"AND {NullQueryParam(ofqualOrganisation, p => p.HeadOfficeAddressCountry)} " +
                   $"AND {NullQueryParam(ofqualOrganisation, p => p.HeadOfficeAddressTelephone)} " +
                    "AND OfqualStatus = @ofqualStatus " +
                    "AND OfqualRecognisedFrom = @ofqualRecognisedFrom " +
                   $"AND {NullQueryParam(ofqualOrganisation, p => p.OfqualRecognisedTo)} " +
                    "AND CreatedAt = @createdAt " +
                   $"AND {NullQueryParam(ofqualOrganisation, p => p.UpdatedAt)} ";

            return await DatabaseService.QueryFirstOrDefaultAsync<OfqualOrganisationModel, OfqualOrganisationModel>(sqlToQuery, ofqualOrganisation);
        }

        public static void DeleteAllRecords()
        {
            var sql = "DELETE FROM [OfqualOrganisation]";

            DatabaseService.Execute(sql);
        }
    }
}
