using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using SFA.DAS.AssessorService.Application.Handlers.Staff;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.Staff
{
    public class StaffIlrRepository : IStaffIlrRepository
    {
        private readonly AssessorDbContext _context;
        private readonly IIlrRepository _ilrRepository;
        private readonly IDbConnection _connection;

        public StaffIlrRepository(AssessorDbContext context, IIlrRepository ilrRepository, IDbConnection connection)
        {
            _context = context;
            _ilrRepository = ilrRepository;
            _connection = connection;
        }

        public async Task<IEnumerable<Ilr>> SearchForLearnerByCertificateReference(string certRef)
        {
            var cert = await _context.Certificates.FirstOrDefaultAsync(c => c.CertificateReference == certRef);
            IEnumerable<Ilr> results =
                cert != null
                    ? new List<Ilr> { await _ilrRepository.Get(cert.Uln, cert.StandardCode) }
                    : new List<Ilr>();

            return results;
        }

        public async Task<IEnumerable<Ilr>> SearchForLearnerByUln(StaffSearchRequest searchRequest)
        {
            long.TryParse(searchRequest.SearchQuery, out var uln);
            /* get rows from Certificates and/or Ilrs  - there must be a better way! - and it does not need paging as there will never be as many as 10 rows*/
            return (await _connection.QueryAsync<Ilr>(
                            @"SELECT [Id] ,[Uln] ,[GivenNames] ,[FamilyName] ,[UkPrn] ,[StdCode] ,[LearnStartDate] ,[EpaOrgId] ,[FundingModel] ,[ApprenticeshipId] ,
                                     [EmployerAccountId] ,[Source] ,[CreatedAt] ,[UpdatedAt] ,[LearnRefNumber] ,[CompletionStatus] ,[EventId] ,[PlannedEndDate]
                            FROM (
                            SELECT [Id] ,[Uln] ,[GivenNames] ,[FamilyName] ,[UkPrn] ,[StdCode] ,[LearnStartDate] ,[EpaOrgId] ,[FundingModel] ,[ApprenticeshipId] ,
                                   [EmployerAccountId] ,[Source] ,[CreatedAt] ,[UpdatedAt] ,[LearnRefNumber] ,[CompletionStatus] ,[EventId] ,[PlannedEndDate],
                             row_number() OVER (PARTITION BY uln,Stdcode ORDER BY choice) rownumber2
                            FROM (
                            SELECT [Id] ,[Uln] ,[GivenNames] ,[FamilyName] ,[UkPrn] ,[StdCode] ,[LearnStartDate] ,[EpaOrgId] ,[FundingModel] ,[ApprenticeshipId] ,
                                  [EmployerAccountId] ,[Source] ,[CreatedAt] ,[UpdatedAt] ,[LearnRefNumber] ,[CompletionStatus] ,[EventId] ,[PlannedEndDate], 2 choice
                            FROM (
                            SELECT Row_number() OVER (ORDER BY certificatereferenceid DESC) rownumber, 
                            ce.id,Uln,
                            json_value(certificatedata, '$.LearnerFamilyName') FamilyName,
                            json_value(certificatedata, '$.LearnerGivenNames') GivenNames,
                            ProviderUkPrn Ukprn,
                            Standardcode Stdcode, 
                            json_value(certificatedata, '$.LearningStartDate') LearnStartDate,
                            og.EndPointAssessorOrganisationId EPAOrgId,
                            NULL FundingModel,
                            NULL ApprenticeshipId,
                            NULL EmployerAccountId,
                            NULL [Source],
                            ce.CreatedAt,
                            ce.UpdatedAt,
                            [LearnRefNumber],
                            1 [CompletionStatus],
                            NULL [EventId],
                            NULL [PlannedEndDate]
                            FROM [Certificates] ce 
                            JOIN [Organisations] og ON ce.OrganisationId = og.Id
                            WHERE ce.[Status] <> 'Deleted'
                            AND [Uln] = @uln 
                            ) ab1 WHERE rownumber = 1
                            UNION
                            SELECT *, 4 choice
                            FROM [Ilrs] 
                            WHERE [Uln] = @uln 
                            ) ab2 
                            ) ab3 WHERE rownumber2 = 1 
                            OFFSET @skip ROWS 
                            FETCH NEXT @take ROWS ONLY",
                new { uln, skip = (searchRequest.Page - 1) * 10, take = 10 })).ToList();
        }

        public async Task<IEnumerable<Ilr>> SearchForLearnerByName(string learnerName, int page, int pageSize)
        {
            var result = await _connection.QueryAsync<Ilr>("StaffSearchCertificates",
                new { Search = learnerName, Skip = (page - 1) * 10, Take = 10 },
                commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<int> CountLearnersByName(string learnerName)
        {
            var deSpacedLearnerName = learnerName.Replace(" ", "");
            return await _context.Ilrs
                .CountAsync(i => i.FamilyName.Replace(" ", "") == deSpacedLearnerName ||
                                 i.GivenNames.Replace(" ", "") == deSpacedLearnerName ||
                   i.GivenNames.Replace(" ", "") + i.FamilyName.Replace(" ", "") == deSpacedLearnerName);
        }

        public async Task<StaffReposSearchResult> SearchForLearnerByEpaOrgId(StaffSearchRequest searchRequest)
        {
            var searchResult = new StaffReposSearchResult
            {
                PageOfResults = (await _connection.QueryAsync<Ilr>(
                        @"SELECT org.EndPointAssessorOrganisationId, cert.Uln, JSON_VALUE(CertificateData, '$.LearnerGivenNames') AS GivenNames, JSON_VALUE(CertificateData, '$.LearnerFamilyName') AS FamilyName, cert.StandardCode AS StdCode, cert.UpdatedAt 
                            FROM Certificates cert
                            INNER JOIN Organisations org ON org.Id = cert.OrganisationId
                            WHERE org.EndPointAssessorOrganisationId = @epaOrgId
                            ORDER BY cert.UpdatedAt DESC                     
                            OFFSET @skip ROWS 
                            FETCH NEXT @take ROWS ONLY",
                        new { epaOrgId = searchRequest.SearchQuery.ToLower(), skip = (searchRequest.Page - 1) * 10, take = 10 }))
                    .ToList(),
                TotalCount = await _connection.ExecuteScalarAsync<int>(@"SELECT COUNT(1)
                    FROM Certificates cert
                        INNER JOIN Organisations org ON org.Id = cert.OrganisationId
                    WHERE org.EndPointAssessorOrganisationId = @epaOrgId", new { epaOrgId = searchRequest.SearchQuery.ToLower() })
            };



            return searchResult;
        }
    }
}