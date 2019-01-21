using System.Collections.Generic;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public class OrganisationStandardDeliveryAreaHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(OrganisationStandardDeliveryAreaModel osDeliveryArea)
        {
            var sql =
                "set identity_insert [OrganisationStandardDeliveryArea] ON; INSERT INTO [dbo].[OrganisationStandardDeliveryArea] ([Id],[OrganisationStandardId],[DeliveryAreaId], " +
                "[Comments],[Status]) VALUES (@id, @organisationStandardId, @deliveryAreaId, @comments, @status); set identity_insert [OrganisationStandardDeliveryArea] OFF; ";
        
            DatabaseService.Execute(sql, osDeliveryArea);
        }
        
        public static void InsertRecords(List<OrganisationStandardDeliveryAreaModel> deliveryAreas)
        {
            foreach (var deliveryArea in deliveryAreas)
            {
                InsertRecord(deliveryArea);
            }
        }
        
        
        public static void DeleteRecord(int id)
        {
            var idToDelete = SqlStringService.ConvertStringToSqlValueString(id.ToString());
            var sql = $@"DELETE from OrganisationStandardDeliveryArea where id = {idToDelete}";
            DatabaseService.Execute(sql);
        }
        
        public static void DeleteRecords(List<int> ids)
        {
            foreach (var id in ids)
            {
                DeleteRecord(id);
            }
        }
    }
}