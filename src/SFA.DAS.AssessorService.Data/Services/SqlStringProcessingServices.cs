using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Data.Services
{
    public class SqlStringProcessingService: ISqlStringProcessingService    {
        public string ConvertStringToSqlValueString(string stringToProcess)
        {
            return stringToProcess == null
                ? "null"
                : $@"'{stringToProcess.Replace("'", "''")}'";
        }

        public string MakeStringSuitableForJson(string stringToProcess)
        {
            return stringToProcess == null
                ? "null"
                : $@"{stringToProcess.Replace("'", "''")}";
        }

        public string ConvertLongToSqlValueString(long? longToProcess)
        {
            return longToProcess == null
                ? "null"
                : $@"{longToProcess}";
        }

        public string ConvertIntToSqlValueString(int? intToProcess)
        {
            return intToProcess == null
                ? "null"
                : $@"{intToProcess}";
        }
    }

    public interface ISqlStringProcessingService
    {
        string ConvertStringToSqlValueString(string stringToProcess);
        string MakeStringSuitableForJson(string stringToProcess);
        string ConvertLongToSqlValueString(long? longToProcess);
        string ConvertIntToSqlValueString(int? intToProcess);
    }
}
