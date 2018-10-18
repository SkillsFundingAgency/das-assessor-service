using Dapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data
{
    public class IlrRepository : IIlrRepository
    {
        private readonly AssessorDbContext _context;
        private readonly IDbConnection _connection;

        public IlrRepository(AssessorDbContext context, IDbConnection connection)
        {
            _context = context;
            _connection = connection;
        }

        public async Task<IEnumerable<Ilr>> SearchForLearnerByUln(long uln)
        {
            return (await _connection.QueryAsync<Ilr>(
                            @"SELECT ilr.*
                            	FROM
	                            (
		                            SELECT *,
		                            DENSE_RANK() OVER (PARTITION BY [Uln], [StdCode] ORDER BY [Source] DESC,[LearnStartDate] DESC) AS rownumber
		                            FROM ilrs 
		                            WHERE [Uln] = @uln
	                            ) AS ilr
                            WHERE rownumber = 1
                            ORDER BY [Id] DESC",
                            new { uln })).ToList();
        }

        public async Task<Ilr> Get(long uln, int standardCode)
        {
            return await _context.Ilrs.FirstAsync(i => i.Uln == uln && i.StdCode == standardCode);
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