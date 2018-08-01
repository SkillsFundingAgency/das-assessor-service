using System;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Models
{
    public class OrganisationModel
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string EndPointAssessorName { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public int? EndPointAssessorUkprn { get; set; }
        public string PrimaryContact { get; set; }
        public string Status { get; set; }
        public DateTime? UpdatedAt { get;set; }
        public int? OrganisationTypeId { get; set; }
        public string OrganisationData { get; set; }
    }
}
