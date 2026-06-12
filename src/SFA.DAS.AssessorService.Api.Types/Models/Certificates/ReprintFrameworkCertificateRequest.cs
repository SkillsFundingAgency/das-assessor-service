using System;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Enums;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class ReprintFrameworkCertificateRequest : IRequest<FrameworkCertificate>
    {
        public Guid FrameworkLearnerId { get; set; }
        public string Username { get; set; }
        public string IncidentNumber { get; set; }
        public ReprintReasons? Reasons { get; set; }
        public string OtherReason { get; set; }

        public string ContactName { get; set; }
        public string ContactAddLine1 { get; set; }
        public string ContactAddLine2 { get; set; }
        public string ContactAddLine3 { get; set; }
        public string ContactAddLine4 { get; set; }
        public string ContactPostcode { get; set; }
    }
}