using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class StartCertificatePrivateRequest : IRequest<Certificate>
    {        
        public long Uln { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
    }
}   