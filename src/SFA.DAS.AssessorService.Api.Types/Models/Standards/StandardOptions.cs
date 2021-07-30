using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.Api.Types.Models.Standards
{
    public class StandardOptions
    {
        public string StandardUId { get; set; }
        public int StandardCode { get; set; }
        public string StandardReference { get; set; }
        public string Version { get; set; }
        public IEnumerable<string> CourseOption { get; set; }

        public bool HasOptions() => CourseOption != null && CourseOption.Any();
        public bool OnlyOneOption() => HasOptions() && CourseOption.Count() == 1;
    }
}
