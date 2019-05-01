namespace SFA.DAS.AssessorService.Api.Types.Models
{
    using System;

    public class OrganisationResponse
    {
        public Guid Id { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public int? EndPointAssessorUkprn { get; set; }
        public string EndPointAssessorName { get; set; }
        public string PrimaryContact { get; set; }

        public string Status { get; set; }
    }
}
