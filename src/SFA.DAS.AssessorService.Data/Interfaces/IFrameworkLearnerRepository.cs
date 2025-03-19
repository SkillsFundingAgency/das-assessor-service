using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.Interfaces
{
    public interface IFrameworkLearnerRepository
    {
        Task<FrameworkLearner> GetFrameworkLearner(Guid frameworkLearnerId);
        Task<IEnumerable<FrameworkLearner>> Search(string firstName, string lastName, DateTime dateOfBirth);
    }
}