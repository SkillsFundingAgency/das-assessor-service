using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class CertificateSearchRequest : IRequest<List<CertificateSearchResponse>>
    {
        public long Uln { get; set; }
        public string Surname { get; set; }
        public string EpaOrgId{ get; set; }
        public string Username { get; set; }
    }
}