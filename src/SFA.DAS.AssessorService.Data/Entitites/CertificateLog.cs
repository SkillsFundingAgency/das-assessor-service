namespace SFA.DAS.AssessorService.Data.Entitites
{
    using System;

    public class CertificateLog : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid CertificateId { get; set; }
        public Certificate Certificate { get; set; }

        public int EndPointAssessorCertificateId { get; set; }
        public string Action { get; set; }
        public string Status { get; set; }
        public DateTime EventTime { get; set; }
    }
}
