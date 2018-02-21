namespace SFA.DAS.AssessorService.Domain.Entities
{
    using System;

    public class CertificateLog : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid CertificateId { get; set; }
        public virtual Certificate Certificate { get; set; }

        public int EndPointAssessorCertificateId { get; set; }
        public string Action { get; set; }
        public CertificateStatus Status { get; set; }
        public DateTime EventTime { get; set; }
    }
}
