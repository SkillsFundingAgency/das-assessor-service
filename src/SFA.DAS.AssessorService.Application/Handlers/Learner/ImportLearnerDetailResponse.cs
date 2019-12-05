using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Handlers.Learner
{
    public class ImportLearnerDetailResponse
    {
        public string Result { get; set; }
        public List<string> Errors { get; set; }
    }
}