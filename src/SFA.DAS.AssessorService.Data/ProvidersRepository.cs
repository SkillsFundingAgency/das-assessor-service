using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data
{
    public class ProvidersRepository : IProvidersRepository
    {
        private readonly IAssessorUnitOfWork _assessorUnitOfWork;

        public ProvidersRepository(IAssessorUnitOfWork assessorUnitOfWork)
        {
            _assessorUnitOfWork = assessorUnitOfWork;
        }

        public async Task<Provider> GetProvider(long ukprn)
        {
            return await _assessorUnitOfWork.AssessorDbContext.Providers.FirstOrDefaultAsync(p => p.Ukprn == ukprn);
        }
    }
}
