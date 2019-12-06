using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class ImportLearnerDetailResponse
    {
        public string Result { get; set; }
        public List<string> Errors { get; set; }
    }
}