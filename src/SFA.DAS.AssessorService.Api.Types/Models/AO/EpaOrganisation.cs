using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.AO
{
    public class EpaOrganisation
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string Name { get; set; }
        public string OrganisationId { get; set; }
        public long? Ukprn { get; set; }
        public string PrimaryContact { get; set; }
        public string Status { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? OrganisationTypeId { get; set; }
        public OrganisationData OrganisationData { get; set; }
        public bool ApiEnabled { get; set; }
        public string ApiUser { get; set; }
    }
}
