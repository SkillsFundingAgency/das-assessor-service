using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.AO
{
    public class AuditData
    {
        public string FieldChanged { get; set; }
        public string PreviousValue { get; set; }
        public string CurrentValue { get; set; }
    }
}
