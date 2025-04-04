using System;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class CertificateLog 
    {
        public Guid Id { get; set; }
        public Guid CertificateId { get; set; }
       
        [JsonIgnore]
        public virtual CertificateBase Certificate { get; set; }

        public string Action { get; set; }
        public string Status { get; set; }
        public DateTime EventTime { get; set; }
        public CertificateData CertificateData { get; set; }
        public string Username { get; set; }
        public int? BatchNumber { get; set; }
        public string ReasonForChange { get; set; }
    }
}