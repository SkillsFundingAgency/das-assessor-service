using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.AO
{
    public class AparSummary
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Ukprn { get; set; }
        public DateTime EarliestDateStandardApprovedOnRegister { get; set; }
        public DateTime EarliestEffectiveFromDate { get; set; }
    }
}
