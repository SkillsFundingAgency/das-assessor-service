using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Learners
{
    public class GetBatchLearnerResponse
    {
        public LearnerDetailForExternalApi Learner { get; set; }
        public Certificate Certificate { get; set; }
        public EpaDetails EpaDetails { get; set; }
        public List<string> ValidationErrors { get; set; } = new List<string>();
    }
}
