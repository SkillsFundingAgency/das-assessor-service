using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class GetCertificatesRequest : IRequest<List<CertificateResponse>>
    {
        public List<string> Statuses { get; set; }
    }
}