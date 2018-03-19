using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class Certificate : BaseEntity
    {
        public Guid Id { get; set; }
        public long Uln { get; set; }
        public int StandardCode { get; set; }
        public int ProviderUkPrn { get; set; }
        public Guid OrganisationId { get; set; }

        public string CertificateReference { get; set; }

        public string CertificateData { get; set; }

        public string Status { get; set; }

        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string DeletedBy { get; set; }
    }
}