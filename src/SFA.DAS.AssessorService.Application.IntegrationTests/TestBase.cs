using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Entities;
using System;

namespace SFA.DAS.AssessorService.Application.Api.IntegrationTests
{
    public class TestBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            DatabaseHelper.SetupDatabase();
            SeedOrganisationTypes();
            SeedDeliveryAreas();
        }

        private void SeedOrganisationTypes()
        {
            DatabaseHelper.Execute("SET IDENTITY_INSERT OrganisationType ON; INSERT INTO OrganisationType (Id, Type, Status, TypeDescription, FinancialExempt) VALUES(1, 'Awarding Organisations', 'Live', 'Awarding organisations', 0 ); SET IDENTITY_INSERT OrganisationType OFF;");
        }

        private void SeedDeliveryAreas()
        {
            DatabaseHelper.Execute("SET IDENTITY_INSERT DeliveryArea ON; INSERT INTO DeliveryArea (Id, Area, Status, Ordering) VALUES(1, 'East Midlands', 'Live', 4 ); SET IDENTITY_INSERT DeliveryArea OFF;");
            DatabaseHelper.Execute("SET IDENTITY_INSERT DeliveryArea ON; INSERT INTO DeliveryArea (Id, Area, Status, Ordering) VALUES(2, 'East of England', 'Live', 6 ); SET IDENTITY_INSERT DeliveryArea OFF;");
            DatabaseHelper.Execute("SET IDENTITY_INSERT DeliveryArea ON; INSERT INTO DeliveryArea (Id, Area, Status, Ordering) VALUES(3, 'London', 'Live', 7 ); SET IDENTITY_INSERT DeliveryArea OFF;");
            DatabaseHelper.Execute("SET IDENTITY_INSERT DeliveryArea ON; INSERT INTO DeliveryArea (Id, Area, Status, Ordering) VALUES(4, 'North East', 'Live', 1 ); SET IDENTITY_INSERT DeliveryArea OFF;");
            DatabaseHelper.Execute("SET IDENTITY_INSERT DeliveryArea ON; INSERT INTO DeliveryArea (Id, Area, Status, Ordering) VALUES(5, 'North West', 'Live', 2 ); SET IDENTITY_INSERT DeliveryArea OFF;");
            DatabaseHelper.Execute("SET IDENTITY_INSERT DeliveryArea ON; INSERT INTO DeliveryArea (Id, Area, Status, Ordering) VALUES(6, 'South East', 'Live', 8 ); SET IDENTITY_INSERT DeliveryArea OFF;");
            DatabaseHelper.Execute("SET IDENTITY_INSERT DeliveryArea ON; INSERT INTO DeliveryArea (Id, Area, Status, Ordering) VALUES(7, 'South West', 'Live', 9 ); SET IDENTITY_INSERT DeliveryArea OFF;");
            DatabaseHelper.Execute("SET IDENTITY_INSERT DeliveryArea ON; INSERT INTO DeliveryArea (Id, Area, Status, Ordering) VALUES(8, 'West Midlands', 'Live', 5 ); SET IDENTITY_INSERT DeliveryArea OFF;");
            DatabaseHelper.Execute("SET IDENTITY_INSERT DeliveryArea ON; INSERT INTO DeliveryArea (Id, Area, Status, Ordering) VALUES(9, 'Yorkshire and the Humber', 'Live', 3 ); SET IDENTITY_INSERT DeliveryArea OFF;");
        }

        protected void CreateOrganisationWithContact(string epaId, string epaName, Guid contactId, string contactName, string contactEmail)
        {
            var orgId = Guid.NewGuid();
            DatabaseHelper.Execute($"INSERT INTO Organisations (Id, CreatedAt, DeletedAt, EndPointAssessorName, EndpointAssessorOrganisationId, EndPointAssessorUkprn, PrimaryContact, Status, UpdatedAt, OrganisationTypeId, OrganisationData, ApiEnabled, ApiUser) VALUES ('{orgId}', '{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}', NULL, '{epaName}', '{epaId}', '10004375', '{contactEmail}', 'Live', '{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}', 1, '{{}}', 0, NULL);");
            DatabaseHelper.Execute($"INSERT INTO Contacts (Id, CreatedAt, DeletedAt, DisplayName, Email, EndPointAssessorOrganisationId, OrganisationId, Status, UpdatedAt, Username, PhoneNumber, Title, GivenNames, FamilyName, SignInType, GovUkIdentifier) VALUES ('{contactId}', '{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}', NULL, '{contactName}', '{contactEmail}', '{epaId}', '{orgId}', 'Live', NULL, '{contactEmail}', '01234 567890', 'Mr', 'Test', 'Contact', '', NULL);");
        }

        protected int CreateOrganisationStandard(string epaId, int standardCode, string standardReference, Guid contactId)
        {
            return DatabaseHelper.ExecuteScaler<int>($"INSERT INTO OrganisationStandard (EndpointAssessorOrganisationId, StandardCode, EffectiveFrom, EffectiveTo, DateStandardApprovedOnRegister, Comments, Status, ContactId, OrganisationStandardData, StandardReference) VALUES ('{epaId}', {standardCode}, '{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}', NULL, '{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}', NULL, 'Live', '{contactId}', '{{}}', '{standardReference}'); SELECT SCOPE_IDENTITY();");
        }

        protected void CreateOrganisationStandardVersion(int organisationStandardId, string standardUid, string version)
        {
            DatabaseHelper.Execute($"INSERT INTO OrganisationStandardVersion (StandardUid, Version, OrganisationStandardId, EffectiveFrom, EffectiveTo, DateVersionApproved, Comments, Status) VALUES ('{standardUid}', {version}, {organisationStandardId}, '{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}', NULL, '{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}', NULL, 'Live');");
        }

        protected void CreateOrganisationStandardDeliveryArea(int organisationStandardId, int deliveryAreaId)
        {
            DatabaseHelper.Execute($"INSERT INTO OrganisationStandardDeliveryArea (OrganisationStandardId, DeliveryAreaId, Comments, Status) VALUES ({organisationStandardId}, {deliveryAreaId},  NULL, 'Live');");
        }

        protected Organisation GetOrganisationByEPAOId(string id)
        {
            var org = DatabaseHelper.Get<Organisation>($"SELECT id, EndPointAssessorOrganisationId, EndpointAssessorName FROM Organisations WHERE EndPointAssessorOrganisationId = '{id}';");
            return org;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            //DatabaseHelper.DropDatabase();
        }
    }
}
