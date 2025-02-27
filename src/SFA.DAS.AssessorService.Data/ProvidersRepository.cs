using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data
{
    public class ProvidersRepository : IProvidersRepository
    {
        private readonly AssessorDbContext _assessorDbContext;

        public ProvidersRepository(AssessorDbContext assessorDbContext)
        {
            _assessorDbContext = assessorDbContext;
        }

        public async Task<Provider> GetProvider(long ukprn)
        {
            return await _assessorDbContext.Providers.FirstOrDefaultAsync(p => p.Ukprn == ukprn);
        }
    }
}
