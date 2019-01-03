using System.Collections.Generic;
using MediatR;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class GetToBeApprovedCertificatesRequest : IRequest<PaginatedList<CertificateSummaryResponse>>
    {
        public GetToBeApprovedCertificatesRequest()
        {
        }

        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public string Status { get; set; }
        public string PrivatelyFundedStatus { get; set; }
    }
}