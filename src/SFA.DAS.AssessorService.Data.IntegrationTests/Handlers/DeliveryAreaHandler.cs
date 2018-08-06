using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public static class DeliveryAreaHandler
    {
        public static void InsertRecord(int id, string area, string status)
        {
            var databaseService = new DatabaseService();

            var sql =
                $@"set identity_insert [DeliveryArea] ON; INSERT INTO [DeliveryArea] ([id], [Area],[Status]) VALUES ({id}, '{area}', '{status}'); set identity_insert[DeliveryArea] OFF; ";

            databaseService.Execute(sql);
        }
    }
}
