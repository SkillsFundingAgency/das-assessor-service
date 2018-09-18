using System.Collections.Generic;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public static class DeliveryAreaHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(DeliveryAreaModel deliveryArea)
        {
            var sqlToInsert = "set identity_insert [DeliveryArea] ON; INSERT INTO [DeliveryArea] ([Id] ,[Area],[Status]) VALUES (@id,@Area, @Status); set identity_insert [DeliveryArea] OFF; ";
            DatabaseService.Execute(sqlToInsert, deliveryArea);
        }

        public static void InsertRecords(List<DeliveryAreaModel> deliveryAreas)
        {
            foreach (var deliveryArea in deliveryAreas)
            {
                InsertRecord(deliveryArea);
            }
        }

        public static void DeleteRecord(int idToDelete)
        {
            var sql = $@"DELETE from DeliveryArea where id = {idToDelete}; ";
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
