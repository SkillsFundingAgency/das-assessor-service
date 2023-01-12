using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class GetCertificateRequest : IRequest<Certificate>
    {
        public GetCertificateRequest(Guid certificateId, bool includeLogs)
        {
            CertificateId = certificateId;
            IncludeLogs = includeLogs;
        }

        public Guid CertificateId { get; }

        public bool IncludeLogs { get; }
    }
}