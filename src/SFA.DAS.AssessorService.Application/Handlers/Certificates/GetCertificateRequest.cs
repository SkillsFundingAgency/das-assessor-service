using System;
using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetCertificateRequest : IRequest<Certificate>
    {
        public long Uln { get; set; }
        public int StandardCode { get; set; }
        public Guid OrganisationId { get; set; }
        public string Username { get; set; }
    }
}   