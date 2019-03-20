using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.AO
{
    public class OrganisationStandard
    {
        public int Id { get; set; }
        public string OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public string OrganisationStatus { get; set; }

        public long? Ukprn { get; set; }
        public DateTime? StandardEffectiveFrom { get; set; }
        public DateTime? StandardEffectiveTo { get; set; }
        public DateTime? StandardLastDateForNewStarts { get; set; }

        public int StandardId { get; set; }
        public string StandardTitle { get; set; }
        public int StandardLevel { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? DateStandardApprovedOnRegister { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public Guid? ContactId { get; set; }
        public EpaContact Contact { get; set; }
        public List<int> DeliveryAreas { get; set; }
        public List<OrganisationStandardDeliveryArea> DeliveryAreasDetails { get; set; }

        public OrganisationStandardData OrganisationStandardData { get; set; }
    }
}