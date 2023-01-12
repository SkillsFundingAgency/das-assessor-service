using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class UpdateCertificateRequestReprintCommand : IRequest<Certificate>
    {
        public Guid CertificateId { get; set; }

        public string Username { get; set; }
    }
}