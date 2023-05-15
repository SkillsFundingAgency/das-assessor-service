using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public static class CertificateLogsHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void DeleteRecord(Guid certificateId)
        {
            var idToDelete = SqlStringService.ConvertGuidToSqlValueString(certificateId);
            var sql = $@"DELETE from CertificateLogs where [CertificateId] = {idToDelete}";
            DatabaseService.Execute(sql);
        }
    }
}
