namespace SFA.DAS.AssessorService.Application.Domain
{
    using System;
    using AssessorService.Domain.Consts;

    public class OrganisationQueryDomainModel
    {
        public Guid Id { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
     
        public string EndPointAssessorName { get; set; }
        public string PrimaryContact { get; set; }
        public OrganisationStatus OrganisationStatus { get; set; }
    }
}
