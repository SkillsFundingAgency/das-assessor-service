using System;
using Dapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.ExternalApis.SubmissionEvents.Types;

namespace SFA.DAS.AssessorService.Data
{
    public class IlrRepository : IIlrRepository
    {
        private readonly AssessorDbContext _context;
        private readonly IDbConnection _connection;
        private readonly string _learnerIlrQueryByUlnFamilyNameFilter = @" WHERE [Uln] = @uln and [FamilyName] = @familyName ";
        private readonly string _learnerIlrQueryByUlnFilter = @" WHERE [Uln] = @uln ";

        public IlrRepository(AssessorDbContext context, IDbConnection connection)
        {
            _context = context;
            _connection = connection;
        }

        public async Task<IEnumerable<Ilr>> SearchForLearnerByUln(long uln)
        {
            return (await _connection.QueryAsync<Ilr>(
                createILRQuery(_learnerIlrQueryByUlnFilter),
                new {uln})).ToList();
        }

        public async Task<IEnumerable<Ilr>> SearchForLearnerByUlnAndFamilyName(long uln, string familyName)
        {
            return (await _connection.QueryAsync<Ilr>(
                createILRQuery(_learnerIlrQueryByUlnFamilyNameFilter),
                new {uln, familyName})).ToList();
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

        public async Task RefreshFromSubmissionEventData(Guid id, SubmissionEvent subEvent)
        {
            var learnerRecord = await _context.Ilrs.FirstAsync(i => i.Id == id);
            if(learnerRecord.EventId != subEvent.Id) learnerRecord.EventId = subEvent.Id;
            if(learnerRecord.UkPrn != (int)subEvent.Ukprn) learnerRecord.UkPrn = (int)subEvent.Ukprn;
            if(learnerRecord.FamilyName != subEvent.FamilyName) learnerRecord.FamilyName = subEvent.FamilyName;
            if(learnerRecord.ApprenticeshipId != subEvent.ApprenticeshipId) learnerRecord.ApprenticeshipId = subEvent.ApprenticeshipId;
            if(learnerRecord.GivenNames != subEvent.GivenNames) learnerRecord.GivenNames = subEvent.GivenNames;
            if(learnerRecord.EpaOrgId != subEvent.EPAOrgId) learnerRecord.EpaOrgId = subEvent.EPAOrgId;
            learnerRecord.Source = ContactStatus.Live;
            learnerRecord.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public async Task MarkAllUpToDate(long uln)
        {
            var learnerRecords = await _context.Ilrs.Where(i => i.Uln == uln).ToListAsync();
            foreach (var learnerRecord in learnerRecords)
            {
                learnerRecord.UpdatedAt = DateTime.Now;
            }
            await _context.SaveChangesAsync();
        }
        private string createILRQuery(string filter)
        {
            return @"SELECT ilr.*
                 FROM
	                (
		                SELECT *,
		                ROW_NUMBER() OVER (PARTITION BY [Uln], [StdCode] ORDER BY [EventId] DESC, [Source] DESC, [CreatedAt] DESC, [LearnStartDate] DESC ) AS rownumber
		                FROM ilrs" +
                   filter
                   +
                   @") AS ilr
                    WHERE rownumber = 1
                    ORDER BY [Id] DESC";
        }


    }
}
