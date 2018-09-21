using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class CreateSequenceResponse
    {
        public bool IsValid { get; set; }
        public SequenceSummary Sequence { get; set; }
        public List<string> Errors { get; set; }
    }
}