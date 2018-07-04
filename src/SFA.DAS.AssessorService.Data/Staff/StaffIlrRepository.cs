using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.Staff
{
    public class StaffIlrRepository : IStaffIlrRepository
    {
        private readonly AssessorDbContext _context;
        private readonly IIlrRepository _ilrRepository;

        public StaffIlrRepository(AssessorDbContext context, IIlrRepository ilrRepository)
        {
            _context = context;
            _ilrRepository = ilrRepository;
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
                .Skip(page * pageSize)
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
    }
}