using System;
using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class StaffCertificateDuplicateRequest : IRequest<Certificate>
    {        
        public Guid Id { get; set;  }
        public string Username { get; set; }
    }
}