using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IIlrRepository
    {
        Task<Ilr> Get(long uln, int stdCode);

        Task Create(Ilr ilr);

        Task Update(Ilr ilr);
    }
}