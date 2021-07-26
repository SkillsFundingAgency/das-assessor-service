using SFA.DAS.AssessorService.Application.Api.External.Models.Request.Certificates;
using SFA.DAS.AssessorService.Application.Api.External.Models.Request.Epa;

namespace SFA.DAS.AssessorService.Application.Api.External.Models.Request
{
    public class CreateEpaRequest
    {
        public string RequestId { get; set; }
        public Standard Standard { get; set; }
        public Learner Learner { get; set; }
        public LearningDetails LearningDetails { get; set; }
        public EpaDetails EpaDetails { get; set; }
    }
}
