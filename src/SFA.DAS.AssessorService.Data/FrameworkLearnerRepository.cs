using Dapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data
{
    public class FrameworkLearnerRepository : IFrameworkLearnerRepository
    {
        private readonly AssessorDbContext _assessorDbContext;

        public FrameworkLearnerRepository(AssessorDbContext assessorDbContext)
        {
            _assessorDbContext = assessorDbContext;
        }

        public async Task<IEnumerable<FrameworkLearner>> Search(string firstName, string lastName, DateTime dateOfBirth)
        {
            return await _assessorDbContext.FrameworkLearners.Where(l => 
                l.ApprenticeForename == firstName && 
                l.ApprenticeSurname == lastName && 
                l.ApprenticeDoB == dateOfBirth)
                    .ToListAsync();
        }

        public async Task<FrameworkLearner> GetById(Guid frameworklearnerId)
        {
            return await _assessorDbContext.FrameworkLearners.FirstOrDefaultAsync(l => l.Id == frameworklearnerId);
        }
    }
}