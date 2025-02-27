using System;
using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class StartFrameworkCertificateRequest : IRequest<FrameworkCertificate>
    {
        public Guid FrameworkLearnerId { get; set; }
        public string Username { get; set; }
    }
}