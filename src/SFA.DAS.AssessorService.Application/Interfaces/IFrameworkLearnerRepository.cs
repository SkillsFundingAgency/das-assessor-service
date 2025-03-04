using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IFrameworkLearnerRepository
    {
        Task<IEnumerable<FrameworkLearner>> Search(string firstName, string lastName, DateTime dateOfBirth);
    }
}
