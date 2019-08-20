using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Learners;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Api.External.Models.Response
{
    public class GetLearnerResponse
    {
        public GetLearner Learner { get; set; }

        public List<string> ValidationErrors { get; set; } = new List<string>();
    }
}
