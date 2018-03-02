namespace SFA.DAS.AssessorService.Application.Domain
{
    using AssessorService.Domain.Enums;

    public class OrganisationCreateDomainModel
    {
        public string EndPointAssessorOrganisationId { get; set; }
        public int EndPointAssessorUkprn { get; set; }
        public string EndPointAssessorName { get; set; }
        public string PrimaryContact { get; set; }

        public OrganisationStatus OrganisationStatus { get; set; }
    }
}
