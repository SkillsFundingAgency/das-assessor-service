using System;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Services
{
    public static class SqlStringService
    {

        public static string ConvertStringToSqlValueString(string stringToProcess)
        {
            return stringToProcess == null
                ? "null"
                : $@"'{stringToProcess.Replace("'", "''")}'";
        }

        public static string MakeStringSuitableForJson(string stringToProcess)
        {
            return stringToProcess == null
                ? "null"
                : $@"{stringToProcess.Replace("'", "''")}";
        }

        public static string ConvertIntToSqlValueString(int? intToProcess)
        {
            return intToProcess == null
                ? "null"
                : $@"{intToProcess}";
        }

        public static string ConvertDateToSqlValueString(DateTime? dateToProcess)
        {
            return dateToProcess == null
                ? "null"
                : $"'{dateToProcess.Value:yyyy-MM-dd}'";
        }
    }
}
