using System;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Domain.DomainModels
{
    public class OrganisationQueryDomainModel
    {
        public Guid Id { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
     
        public string EndPointAssessorName { get; set; }
        public string PrimaryContact { get; set; }
        public OrganisationStatus OrganisationStatus { get; set; }
    }
}
