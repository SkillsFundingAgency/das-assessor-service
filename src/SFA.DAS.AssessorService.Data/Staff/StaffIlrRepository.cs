using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
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
                    ? new List<Ilr> {await _ilrRepository.Get(cert.Uln, cert.StandardCode)} 
                    : new List<Ilr>();

            return results;
        }

        public async Task<IEnumerable<Ilr>> SearchForLearnerByName(string learnerName, int page, int pageSize)
        {
            var deSpacedLearnerName = learnerName.Replace(" ", "");
            return await _context.Ilrs.Where(i =>
                    i.FamilyName.Replace(" ", "") == deSpacedLearnerName ||
                    i.GivenNames.Replace(" ", "") == deSpacedLearnerName ||
                    i.GivenNames.Replace(" ", "") + i.FamilyName.Replace(" ", "") == deSpacedLearnerName)
                .OrderBy(i => i.FamilyName)
                .ThenBy(i => i.GivenNames)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountLearnersByName(string learnerName)
        {   
            var deSpacedLearnerName = learnerName.Replace(" ", "");
            return await _context.Ilrs
                .CountAsync(i => i.FamilyName.Replace(" ", "") == deSpacedLearnerName ||
                                 i.GivenNames.Replace(" ", "") == deSpacedLearnerName ||
                   i.GivenNames.Replace(" ", "") + i.FamilyName.Replace(" ", "") == deSpacedLearnerName);
        }

        public async Task<IEnumerable<Ilr>> SearchForLearnerByEpaOrgId(StaffSearchRequest searchRequest)
        {
            return (await _connection.QueryAsync<Ilr>(@"SELECT ilr.Uln, ilr.GivenNames, ilr.FamilyName, ilr.StdCode, cert.UpdatedAt 
		FROM Certificates cert
        INNER JOIN Organisations org ON org.Id = cert.OrganisationId
        INNER JOIN Ilrs ilr ON ilr.Uln = cert.Uln AND ilr.StdCode = cert.StandardCode
        WHERE org.EndPointAssessorOrganisationId = @epaOrgId
		ORDER BY cert.UpdatedAt DESC
		OFFSET @skip ROWS 
		FETCH NEXT @take ROWS ONLY", new {epaOrgId = searchRequest.SearchQuery, skip = (searchRequest.Page - 1) * 10, take = 10})).ToList();
        }
    }
}