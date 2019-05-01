using MediatR;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class GetCertificateHistoryRequest : IRequest<PaginatedList<CertificateSummaryResponse>>
    {
        public GetCertificateHistoryRequest()
        {            
        }

        public int? PageIndex { get; set; }
        public string Username { get; set; }
    }
}