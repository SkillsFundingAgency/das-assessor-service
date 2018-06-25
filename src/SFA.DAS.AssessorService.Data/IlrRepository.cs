using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.AssessorService.Data
{
    public class IlrRepository : IIlrRepository
    {
        private readonly AssessorDbContext _context;

        public IlrRepository(AssessorDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ilr>> SearchForLearner(SearchRequest searchRequest)
        {
            return await _context.Ilrs.Where(r => r.Uln == searchRequest.Uln).ToListAsync();
        }

        public async Task<Ilr> Get(long uln, int standardCode)
        {
            return await _context.Ilrs.SingleAsync(i => i.Uln == uln && i.StdCode == standardCode);
        }
    }
}