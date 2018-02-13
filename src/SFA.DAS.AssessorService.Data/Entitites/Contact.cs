namespace SFA.DAS.AssessorService.Data.Entitites
{
    using System;

    public class Contact : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid OrganisationId { get; set; }
        public Organisation Organisation { get; set; }

        public int EndPointAssessorContactId { get; set; }
        public int EndPointAssessorOrganisationId{ get; set; }
        public int EndPointAssessorUKPRN { get; set; }

        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string Status { get; set; }
    }
}
