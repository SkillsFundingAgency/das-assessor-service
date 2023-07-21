using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public class OfqualStandardHandler : HandlerBase
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(OfqualStandardModel ofqualStandard)
        {
            var sqlToInsert =
                "INSERT INTO [dbo].[OfqualStandard]" +
                    "([Id]" +
                    ", [RecognitionNumber]" +
                    ", [OperationalStartDate]" +
                    ", [OperationalEndDate]" +
                    ", [IfateReferenceNumber]" +
                    ", [CreatedAt]" +
                    ", [UpdatedAt]) " +
                "VALUES " +
                    "(@id" +
                    ", @recognitionNumber" +
                    ", @operationalStartDate" +
                    ", @operationalEndDate" +
                    ", @ifateReferenceNumber" +
                    ", @createdAt" +
                    ", @updatedAt)";

            DatabaseService.Execute(sqlToInsert, ofqualStandard);
        }

        public static void InsertRecords(List<OfqualStandardModel> ofqualStandards)
        {
            foreach (var ofqualStandard in ofqualStandards)
            {
                InsertRecord(ofqualStandard);
            }
        }

        public static OfqualStandardModel Create(Guid? id, string recognitionNumber, DateTime operationalStartDate, DateTime? operationalEndDate, string ifateReferenceNumber,
            DateTime createdAt, DateTime? updatedAt)
        {
            return new OfqualStandardModel
            {
                Id = id,
                RecognitionNumber = recognitionNumber,
                OperationalStartDate = operationalStartDate,
                OperationalEndDate = operationalEndDate,
                IfateReferenceNumber = ifateReferenceNumber,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };
        }

        public static async Task<OfqualStandardModel> QueryFirstOrDefaultAsync(OfqualStandardModel ofqualStandard)
        {
            var sqlToQuery =
                "SELECT" +
                    "[Id]" +
                    ", [RecognitionNumber]" +
                    ", [OperationalStartDate]" +
                    ", [OperationalEndDate]" +
                    ", [IfateReferenceNumber]" +
                    ", [CreatedAt]" +
                    ", [UpdatedAt]" +
                "FROM [OfqualStandard] " +
                "WHERE (Id = @id OR @id IS NULL) " + // when @id is null then Id is not predicated
                    "AND RecognitionNumber = @recognitionNumber " +
                    "AND OperationalStartDate = @operationalStartDate " +
                   $"AND {NullQueryParam(ofqualStandard, p => p.OperationalEndDate)} " +
                    "AND IfateReferenceNumber = @ifateReferenceNumber " +
                    "AND CreatedAt = @createdAt " +
                   $"AND {NullQueryParam(ofqualStandard, p => p.UpdatedAt)} ";

            return await DatabaseService.QueryFirstOrDefaultAsync(sqlToQuery, ofqualStandard);
        }

        public static void DeleteAllRecords()
        {
            var sql = "DELETE FROM [OfqualStandard]";

            DatabaseService.Execute(sql);
        }
    }
}
