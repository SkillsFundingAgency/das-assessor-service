using SFA.DAS.Apprenticeships.Api.Types.Providers;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Learners
{
    public class GetBatchLearnerResponse
    {
        public Ilr Ilr { get; set; }
        public StandardCollation Standard { get; set; }
        public Provider Provider { get; set; }

        public Certificate Certificate { get; set; }

        public List<string> ValidationErrors { get; set; } = new List<string>();
    }
}
