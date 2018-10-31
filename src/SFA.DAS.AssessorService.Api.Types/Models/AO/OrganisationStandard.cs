using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.AO
{
    public class OrganisationStandard
    {
        public int Id { get; set; }
        public string OrganisationId { get; set; }
        public int StandardCode { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? DateStandardApprovedOnRegister { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public Guid? ContactId { get; set; }
        public EpaContact Contact { get; set; }
        public List<OrganisationStandardDeliveryArea> DeliveryAreas { get; set; }
    }
}