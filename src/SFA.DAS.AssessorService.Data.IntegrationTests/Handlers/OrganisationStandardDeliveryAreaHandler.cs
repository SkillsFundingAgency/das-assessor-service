using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public class OrganisationStandardDeliveryAreaHandler : HandlerBase
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static OrganisationStandardDeliveryAreaModel Create(int id, int organisationStandardId, int deliveryAreaId, string comments, string status)
        {
            return new OrganisationStandardDeliveryAreaModel
            {
                Id = id,
                OrganisationStandardId = organisationStandardId,
                DeliveryAreaId = deliveryAreaId,
                Comments = comments,
                Status = status
            };
        }

        public static void InsertRecord(OrganisationStandardDeliveryAreaModel organisationStandardDeliveryAreaModel)
        {
            var sqlToInsert =
                "SET IDENTITY_INSERT [OrganisationStandardDeliveryArea] ON; " +
                "INSERT INTO [dbo].[OrganisationStandardDeliveryArea] " +
                    "([Id]" +
                    ", [OrganisationStandardId]" +
                    ", [DeliveryAreaId]" +
                    ", [Comments]" +
                    ", [Status]) " +
                "VALUES (@id" +
                    ", @organisationStandardId" +
                    ", @deliveryAreaId" +
                    ", @comments" +
                    ", @status); " +
                "SET IDENTITY_INSERT [OrganisationStandardDeliveryArea] OFF; ";
        
            DatabaseService.Execute(sqlToInsert, organisationStandardDeliveryAreaModel);
        }
        
        public static void InsertRecords(List<OrganisationStandardDeliveryAreaModel> deliveryAreas)
        {
            foreach (var deliveryArea in deliveryAreas)
            {
                InsertRecord(deliveryArea);
            }
        }

        public static async Task<OrganisationStandardDeliveryAreaModel> QueryFirstOrDefaultAsync(OrganisationStandardDeliveryAreaModel organisationStandardDeliveryArea)
        {
            var sqlToQuery =
                "SELECT " +
                    "[Id]" +
                    ", [OrganisationStandardId]" +
                    ", [DeliveryAreaId]" +
                    ", [Comments]" +
                    ", [Status]) " +
                "FROM [OrganisationStandardDeliveryArea] " +
                $"WHERE " +
                    // when Id is null then Id is not predicated
                    $"AND Id = @id OR @id IS NULL " +
                    // when organisationStandardId is null then OrganisationStandardId is not predicated
                    $"AND OrganisationStandardId = @organisationStandardId OR @organisationStandardId IS NULL " +
                    $"AND {NullQueryParam(organisationStandardDeliveryArea, p => p.DeliveryAreaId)} " +
                    $"AND {NullQueryParam(organisationStandardDeliveryArea, p => p.Comments)} " +
                    $"AND {NotNullQueryParam(organisationStandardDeliveryArea, p => p.Status)} ";

            return await DatabaseService.QueryFirstOrDefaultAsync<OrganisationStandardDeliveryAreaModel, OrganisationStandardDeliveryAreaModel>(sqlToQuery, organisationStandardDeliveryArea);
        }

        public static async Task<int> QueryCountAllAsync()
        {
            var sqlToQuery =
                "SELECT COUNT(1)" +
                "FROM [OrganisationStandardDeliveryArea]";

            return await DatabaseService.QueryFirstOrDefaultAsync<int>(sqlToQuery);
        }

        public static void DeleteRecord(int id)
        {
            var sqlToDelete = 
                "DELETE FROM [OrganisationStandardDeliveryArea] WHERE Id = @id";
            
            DatabaseService.Execute(sqlToDelete, new {Id = id});
        }

        public static void DeleteRecords(List<int> ids)
        {
            foreach (var id in ids)
            {
                DeleteRecord(id);
            }
        }

        public static void DeleteAllRecords()
        {
            var sql = $@"DELETE FROM [OrganisationStandardDeliveryArea]";
            DatabaseService.Execute(sql);
        }
    }
}