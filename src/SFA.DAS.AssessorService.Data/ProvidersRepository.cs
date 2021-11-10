using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Threading.Tasks;

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
