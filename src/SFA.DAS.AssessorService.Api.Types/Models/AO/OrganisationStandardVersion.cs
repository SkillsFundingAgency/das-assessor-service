using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.AO
{
    public class OrganisationStandardVersion
    {
        public string StandardUId { get; set; }
        public string Version { get; set; }
        public int OrganisationStandardId { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? DateVersionApproved { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
    }
}