using System.Collections.Generic;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Domain.DTOs.Staff
{
    public class StaffReposSearchResult
    {
        public IEnumerable<Learner> PageOfResults { get; set; }
        public bool DisplayEpao { get; set; }
        public int TotalCount { get; set; }
    }
}
