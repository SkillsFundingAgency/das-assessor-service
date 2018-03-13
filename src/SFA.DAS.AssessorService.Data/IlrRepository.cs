using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Linq;

namespace SFA.DAS.AssessorService.Data
{
    public class IlrRepository : IIlrRepository
    {
        private readonly AssessorDbContext _context;

        public IlrRepository(AssessorDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Ilr> Search(SearchRequest searchRequest)
        {
            var response = _context.Ilrs.Where(r =>
                string.Equals(r.FamilyName.Trim(), searchRequest.FamilyName.Trim(), StringComparison.CurrentCultureIgnoreCase)
                && r.Uln == searchRequest.Uln
                && searchRequest.StandardIds.Contains(r.StdCode));

            return response;
        }
    }

    
}