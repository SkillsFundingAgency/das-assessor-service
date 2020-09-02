using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class CertificateBatchLog : BaseEntity
    {
        public Guid Id { get; set; }
        public string CertificateReference { get; set; }
        public int BatchNumber { get; set; }
        public string CertificateData { get; set; }
        public string Status { get; set; }
        public DateTime StatusAt { get; set; }
        public string ReasonForChange { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string DeletedBy { get; set; }
        public Certificate Certificate { get; set; }
    }
}