using System;

namespace SFA.DAS.AssessorService.Domain.DomainModels
{
    public class CreateOrganisationDomainModel
    {
        public string EndPointAssessorOrganisationId { get; set; }
        public int EndPointAssessorUkprn { get; set; }
        public string EndPointAssessorName { get; set; }
        public string PrimaryContact { get; set; }

        public string Status { get; set; }
    }
}
