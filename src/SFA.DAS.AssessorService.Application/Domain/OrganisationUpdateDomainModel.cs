namespace SFA.DAS.AssessorService.Application.Domain
{
    using System;
    using AssessorService.Domain.Enums;

    public class OrganisationUpdateDomainModel
    {
        public Guid Id { get; set; }
     
        public string EndPointAssessorName { get; set; }
        public Guid? PrimaryContactId { get; set; }
        public OrganisationStatus OrganisationStatus { get; set; }
    }
}
