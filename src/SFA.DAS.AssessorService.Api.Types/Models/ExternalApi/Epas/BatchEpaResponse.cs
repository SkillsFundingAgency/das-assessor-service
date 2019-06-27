using SFA.DAS.AssessorService.Domain.JsonData;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas
{
    public class BatchEpaResponse
    {
        public string RequestId { get; set; }
        public long Uln { get; set; }
        public string FamilyName { get; set; }

        public int StandardCode { get; set; }
        public string StandardReference { get; set; }

        public EpaDetails EpaDetails { get; set; }

        public List<string> ValidationErrors { get; set; } = new List<string>();
    }
}
