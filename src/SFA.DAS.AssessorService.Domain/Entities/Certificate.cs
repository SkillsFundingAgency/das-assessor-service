using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class Certificate : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }

        public int EndPointAssessorCertificateId { get; set; }

        // Will hold json representation for data - change made
        // as per alans request 22-02-2018 JC
        // Data ot serialised using structure defined in CertificateData class.
        public string CertificateData { get; set; }

        public string Status { get; set; }

        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string DeletedBy { get; set; }
    }
}