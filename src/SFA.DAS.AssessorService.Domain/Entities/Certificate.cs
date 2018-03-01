namespace SFA.DAS.AssessorService.Domain.Entities
{
    using Enums;
    using System;

    public class Certificate : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }

        // Will hold json representation for data - change made
        // as per alans request 22-02-2018 JC
        // Data ot serialised using structure defined in CertificateData class.
        public string CertificateData { get; set; }

        public CertificateStatus Status { get; set; }

        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
        public Guid DeletedBy { get; set; }
    }
}
