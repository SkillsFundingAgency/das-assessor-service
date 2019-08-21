using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Api.External.Models.Response
{
    public class CreateEpaResponse
    {
        public string RequestId { get; set; }

        public string EpaReference { get; set; }

        public List<string> ValidationErrors { get; set; } = new List<string>();
    }
}
