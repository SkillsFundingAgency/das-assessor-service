using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

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
    }
}