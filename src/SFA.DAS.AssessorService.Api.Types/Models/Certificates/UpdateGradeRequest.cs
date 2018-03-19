using System;
using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class UpdateGradeRequest : IRequest<Certificate>
    {
        public Guid CertificateId { get; set; }
        public string Grade { get; set; }
    }
}