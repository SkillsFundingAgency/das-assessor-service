using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class GetCertificateHistoryRequest : IRequest<List<CertificateHistoryResponse>>
    {
        public GetCertificateHistoryRequest()
        {            
        }        
    }
}