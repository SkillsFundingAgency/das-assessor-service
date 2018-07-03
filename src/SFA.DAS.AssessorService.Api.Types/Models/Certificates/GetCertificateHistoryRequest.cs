using MediatR;
using SFA.AssessorService.Paging;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class GetCertificateHistoryRequest : IRequest<PaginatedList<CertificateHistoryResponse>>
    {
        public GetCertificateHistoryRequest()
        {            
        }

        public int? PageIndex { get; set; }
        public string Username { get; set; }
    }
}