using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IStaffIlrRepository
    {
        Task<IEnumerable<Ilr>> SearchForLearnerByCertificateReference(string certRef);
        Task<IEnumerable<Ilr>> SearchForLearnerByName(string learnerName, int page, int pageSize);
        Task<int> CountLearnersByName(string learnerName);
        Task<IEnumerable<Ilr>> SearchForLearnerByEpaOrgId(string epaOrgId);
    }
}