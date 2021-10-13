using SFA.DAS.AssessorService.Domain.Entities;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IProvidersRepository
    {
        Task<Provider> GetProvider(long ukprn);
    }
}
