using System;
using System.Collections.Generic;
using SFA.DAS.Apprenticeships.Api.Types;

namespace SFA.DAS.AssessorService.Api.Types.Models.AO
{
    public class OrganisationStandardSummary
    {
        public int Id { get; set; }
        public string OrganisationId { get; set; }
        public int StandardCode { get; set; }

        public DateTime? DateStandardApprovedOnRegister { get; set; }
        public Guid? ContactId { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        public StandardSummary StandardSummary { get; set; }
        public List<int> DeliveryAreas { get; set; }

        public OrganisationStandardData OrganisationStandardData { get; set; }
    }
}
