using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.DTOs.Staff;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.Interfaces
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
}