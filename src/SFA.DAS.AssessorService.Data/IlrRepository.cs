﻿using System;
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

        public async Task<IEnumerable<Ilr>> SearchForLearnerByUln(long uln)
        {
            return await _context.Ilrs.Where(r => r.Uln == uln)
                .GroupBy(r => new{ r.FamilyName, r.StdCode })
                .Select(g => g.OrderByDescending(l => l.Id).First())
                .ToListAsync();
        }

        public async Task<Ilr> Get(long uln, int standardCode)
        {
            return await _context.Ilrs.FirstOrDefaultAsync(i => i.Uln == uln && i.StdCode == standardCode);
        }

        public async Task StoreSearchLog(SearchLog log)
        {
            await _context.SearchLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Ilr>> Search(string searchQuery)
        {
            return await _context.Ilrs.Where(r => r.FamilyName == searchQuery || r.GivenNames == searchQuery || r.Uln == long.Parse(searchQuery))
                .ToListAsync();
        }
    }
}