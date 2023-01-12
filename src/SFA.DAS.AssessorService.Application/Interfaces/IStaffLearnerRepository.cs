using SFA.DAS.AssessorService.Application.Handlers.Staff;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IStaffLearnerRepository
    {
        Task<IEnumerable<Learner>> SearchForLearnerByCertificateReference(string certRef);
        Task<IEnumerable<Learner>> SearchForLearnerByName(string learnerName, int page, int pageSize);
        Task<int> SearchForLearnerByNameCount(string learnerName);
        Task<StaffReposSearchResult> SearchForLearnerByEpaOrgId(StaffSearchRequest searchRequest);
        Task<IEnumerable<Learner>> SearchForLearnerByUln(long uln, int page, int pageSize);
        Task<int> SearchForLearnerByUlnCount(long uln);
    }

    public class StaffReposSearchResult
    {
        public IEnumerable<Learner> PageOfResults { get; set; }
        public bool DisplayEpao { get; set; }
        public int TotalCount { get; set; }
    }
}