using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.AssessorService.Data
{
    public class FrameworkLearnerRepository : IFrameworkLearnerRepository
    {
        private readonly IAssessorUnitOfWork _unitOfWork;

        public FrameworkLearnerRepository(IAssessorUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FrameworkLearner> GetFrameworkLearner(Guid frameworkLearnerId)
        { 
            return await _unitOfWork.AssessorDbContext.FrameworkLearners
                .SingleOrDefaultAsync(p => p.Id == frameworkLearnerId);
        }
        public async Task<IEnumerable<FrameworkLearner>> Search(string firstName, string lastName, DateTime dateOfBirth)
        {
            return await _unitOfWork.AssessorDbContext.FrameworkLearners.Where(l => 
                l.ApprenticeForename == firstName && 
                l.ApprenticeSurname == lastName && 
                l.ApprenticeDoB == dateOfBirth)
                    .ToListAsync();
        }
    }
}