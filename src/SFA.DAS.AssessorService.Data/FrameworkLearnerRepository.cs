using Dapper;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data
{
    public class FrameworkLearnerRepository : Repository, IFrameworkLearnerRepository
    {
        public FrameworkLearnerRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<FrameworkLearner>> Search(string firstName, string lastName, DateTime dateOfBirth)
        {
            return (await _unitOfWork.Connection.QueryAsync<FrameworkLearner>(
               $@"SELECT * FROM [FrameworkLearner] WHERE [ApprenticeForename] = @firstName AND [ApprenticeSurname] = @lastName AND [ApprenticeDoB] = @dateOfBirth",
                 param: new { firstName, lastName, dateOfBirth },
                 transaction: _unitOfWork.Transaction)).ToList();
        }
    }
}