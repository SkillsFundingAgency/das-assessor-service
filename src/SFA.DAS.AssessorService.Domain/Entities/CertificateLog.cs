using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class CertificateLog 
    {
        public Guid Id { get; set; }
        public Guid CertificateId { get; set; }
        public virtual Certificate Certificate { get; set; }

        public string Action { get; set; }
        public string Status { get; set; }
        public DateTime EventTime { get; set; }
        public string CertificateData { get; set; }
        public string Username { get; set; }
        public int? BatchNumber { get; set; }
    }
}