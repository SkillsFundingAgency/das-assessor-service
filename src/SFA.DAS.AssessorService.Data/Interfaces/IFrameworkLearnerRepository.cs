using System;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.Interfaces
{
    public interface IFrameworkLearnerRepository
    {
        Task<FrameworkLearner> GetFrameworkLearner(Guid frameworkLearnerId);
    }
}