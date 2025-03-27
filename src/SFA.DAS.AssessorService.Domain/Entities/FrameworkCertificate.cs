using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class FrameworkCertificate : CertificateBase
    {
        public Guid FrameworkLearnerId { get; set; }
    }
}
