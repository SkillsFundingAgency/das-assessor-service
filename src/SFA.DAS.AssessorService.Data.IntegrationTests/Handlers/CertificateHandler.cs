using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public static class CertificateHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void DeleteRecord(Guid certificateId)
        {
            var idToDelete = SqlStringService.ConvertGuidToSqlValueString(certificateId);
            var sql = $@"DELETE from Certificates where [Id] = {idToDelete}";
            DatabaseService.Execute(sql);
        }
    }
}

