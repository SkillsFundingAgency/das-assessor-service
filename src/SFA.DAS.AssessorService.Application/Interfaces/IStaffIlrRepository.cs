using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Application.Handlers.Staff;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IStaffIlrRepository
    {
        Task<IEnumerable<Ilr>> SearchForLearnerByCertificateReference(string certRef);
        Task<IEnumerable<Ilr>> SearchForLearnerByName(string learnerName, int page, int pageSize);
        Task<int> SearchForLearnerByNameCount(string learnerName);
        Task<StaffReposSearchResult> SearchForLearnerByEpaOrgId(StaffSearchRequest searchRequest);
        Task<IEnumerable<Ilr>> SearchForLearnerByUln(long uln, int page, int pageSize);
        Task<int> SearchForLearnerByUlnCount(long uln);
    }

    public class StaffReposSearchResult
    {
        public IEnumerable<Ilr> PageOfResults { get; set; }
        public bool DisplayEpao { get; set; }
        public int TotalCount { get; set; }
    }
}