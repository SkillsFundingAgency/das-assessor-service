using System;
using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class GetCertificateRequest : IRequest<Certificate>
    {
        public GetCertificateRequest(Guid certificateId)
        {
            CertificateId = certificateId;
        }
        public Guid CertificateId { get; set; }
    }
}