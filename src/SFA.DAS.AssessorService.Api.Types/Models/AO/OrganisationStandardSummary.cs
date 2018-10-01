using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs.Types;
using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.Apprenticeships.Api.Types;

namespace SFA.DAS.AssessorService.Api.Types.Models.AO
{
    public class OrganisationStandardSummary
    {
        public string OrganisationId { get; set; }
        public int StandardCode { get; set; }

        public DateTime? DateStandardApprovedOnRegister { get; set; }
        public Guid? ContactId { get; set; }
       
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        public Standard Standard { get; set; }
    }
}
