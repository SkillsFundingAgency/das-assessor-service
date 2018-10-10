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
            return await _context.Ilrs.Where(r => r.Uln == uln).GroupBy(r => r.StdCode).Select(g => g.OrderByDescending(l => l.Id).First())
                .Skip((searchRequest.Page - 1) * 10)
                .Take(10)
                .ToListAsync();
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
                            INNER JOIN Ilrs ilr ON ilr.Uln = cert.Uln AND ilr.StdCode = cert.StandardCode
                            WHERE org.EndPointAssessorOrganisationId = @epaOrgId
		                    ORDER BY cert.UpdatedAt DESC 		            
		                    OFFSET @skip ROWS 
		                    FETCH NEXT @take ROWS ONLY",
                        new { epaOrgId = searchRequest.SearchQuery.ToLower(), skip = (searchRequest.Page - 1) * 10, take = 10 }))
                    .ToList(),
                TotalCount = await _connection.ExecuteScalarAsync<int>(@"SELECT COUNT(1)
                    FROM Certificates cert
                        INNER JOIN Organisations org ON org.Id = cert.OrganisationId
                    INNER JOIN Ilrs ilr ON ilr.Uln = cert.Uln AND ilr.StdCode = cert.StandardCode
                    WHERE org.EndPointAssessorOrganisationId = @epaOrgId", new { epaOrgId = searchRequest.SearchQuery.ToLower() })
            };



            return searchResult;
        }
    }
}