using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models.Standards
{
    public class StandardVersion
    {
        public string StandardUId { get; set; }
        public string Title { get; set; }
        public List<string> Options { get; set; }
        public string Version { get; set; }
    }
}
