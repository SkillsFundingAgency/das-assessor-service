using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public class CertificateHandler : HandlerBase
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(CertificateModel certificate)
        {
            var sql =
                "SET IDENTITY_INSERT [Certificates] ON; " +
                "INSERT INTO [Certificates] " +
                "([Id], [CertificateData], [ToBePrinted], [CreatedAt], [CreatedBy], [DeletedAt], [DeletedBy], " +
                "[CertificateReference], [OrganisationId], [BatchNumber], [Status], [UpdatedAt], [UpdatedBy], " +
                "[Uln], [StandardCode], [ProviderUkPrn], [CertificateReferenceId], [LearnRefNumber], [CreateDay], " +
                "[IsPrivatelyFunded], [PrivatelyFundedStatus], [StandardUId]) " +
                "VALUES " +
                "(@Id, @CertificateData, @ToBePrinted, @CreatedAt, @CreatedBy, @DeletedAt, @DeletedBy, " +
                "@CertificateReference, @OrganisationId, @BatchNumber, @Status, @UpdatedAt, @UpdatedBy, " +
                "@Uln, @StandardCode, @ProviderUkPrn, @CertificateReferenceId, @LearnRefNumber, @CreateDay, " +
                "@IsPrivatelyFunded, @PrivatelyFundedStatus, @StandardUId);" +
                "SET IDENTITY_INSERT [Certificates] OFF; ";

            DatabaseService.Execute(sql, certificate);
        }

        public static void InsertRecords(List<CertificateModel> certificates)
        {
            foreach (var certificate in certificates)
            {
                InsertRecord(certificate);
            }
        }

        public static CertificateModel Create(
            Guid? id, string certificateData, DateTime? toBePrinted, DateTime createdAt, string createdBy,
            string certificateReference, Guid organisationId, long uln, int standardCode, int providerUkPrn,
            string status, DateTime? updatedAt, string updatedBy, DateTime createDay, int ? batchNumber = null, DateTime? deletedAt = null,
            string deletedBy = null, int certificateReferenceId = 10001, string learnRefNumber = null,
            bool isPrivatelyFunded = false, string privatelyFundedStatus = null,
            string standardUId = null)
        {
            return new CertificateModel
            {
                Id = id ?? Guid.NewGuid(),
                CertificateData = certificateData,
                ToBePrinted = toBePrinted,
                CreatedAt = createdAt,
                CreatedBy = createdBy,
                DeletedAt = deletedAt,
                DeletedBy = deletedBy,
                CertificateReference = certificateReference,
                OrganisationId = organisationId,
                BatchNumber = batchNumber,
                Status = status,
                UpdatedAt = updatedAt,
                UpdatedBy = updatedBy,
                Uln = uln,
                StandardCode = standardCode,
                ProviderUkPrn = providerUkPrn,
                CertificateReferenceId = certificateReferenceId,
                LearnRefNumber = learnRefNumber,
                CreateDay = createDay,
                IsPrivatelyFunded = isPrivatelyFunded,
                PrivatelyFundedStatus = privatelyFundedStatus,
                StandardUId = standardUId
            };
        }

        public static async Task<CertificateModel> QueryFirstOrDefaultAsync(CertificateModel certificate)
        {
            var sqlToQuery =
            "SELECT " +
            "[Id], [CertificateData], [ToBePrinted], [CreatedAt], [CreatedBy], [DeletedAt], [DeletedBy], " +
            "[CertificateReference], [OrganisationId], [BatchNumber], [Status], [UpdatedAt], [UpdatedBy], " +
            "[Uln], [StandardCode], [ProviderUkPrn], [CertificateReferenceId], [LearnRefNumber], [CreateDay], " +
            "[IsPrivatelyFunded], [PrivatelyFundedStatus], [StandardUId] " +
            "FROM [StandardCertificates] " +
            $"WHERE (Id = @id OR @id IS NULL) " +
            $"AND {NullQueryParam(certificate, p => p.CertificateData)} " +
            $"AND {NullQueryParam(certificate, p => p.ToBePrinted)} " +
            $"AND {NullQueryParam(certificate, p => p.CreatedAt)} " +
            $"AND {NullQueryParam(certificate, p => p.CreatedBy)} " +
            $"AND {NullQueryParam(certificate, p => p.DeletedAt)} " +
            $"AND {NullQueryParam(certificate, p => p.DeletedBy)} " +
            $"AND {NullQueryParam(certificate, p => p.CertificateReference)} " +
            $"AND {NullQueryParam(certificate, p => p.OrganisationId)} " +
            $"AND {NullQueryParam(certificate, p => p.BatchNumber)} " +
            $"AND {NullQueryParam(certificate, p => p.Status)} " +
            $"AND {NullQueryParam(certificate, p => p.UpdatedAt)} " +
            $"AND {NullQueryParam(certificate, p => p.UpdatedBy)} " +
            $"AND {NotNullQueryParam(certificate, p => p.Uln)} " +
            $"AND {NotNullQueryParam(certificate, p => p.StandardCode)} " +
            $"AND {NotNullQueryParam(certificate, p => p.ProviderUkPrn)} " +
            $"AND {NotNullQueryParam(certificate, p => p.CertificateReferenceId)} " +
            $"AND {NullQueryParam(certificate, p => p.LearnRefNumber)} " +
            $"AND {NotNullQueryParam(certificate, p => p.CreateDay)} " +
            $"AND {NullQueryParam(certificate, p => p.IsPrivatelyFunded)} " +
            $"AND {NullQueryParam(certificate, p => p.PrivatelyFundedStatus)} " +
            $"AND {NullQueryParam(certificate, p => p.StandardUId)};";

            return await DatabaseService.QueryFirstOrDefaultAsync<CertificateModel>(sqlToQuery, certificate);
        }

        public static async Task<int> QueryCountAllAsync()
        {
            var sqlToQuery = "SELECT COUNT(1) FROM [StandardCertificates]";
            return await DatabaseService.QueryFirstOrDefaultAsync<int>(sqlToQuery);
        }

        public static void DeleteRecord(Guid id)
        {
            var sql = "DELETE FROM [StandardCertificates] WHERE [Id] = @id";
            DatabaseService.Execute(sql, new { id });
        }

        public static void DeleteAllRecords()
        {
            var sql = "DELETE FROM [StandardCertificates]";
            DatabaseService.Execute(sql);
        }
    }
}
