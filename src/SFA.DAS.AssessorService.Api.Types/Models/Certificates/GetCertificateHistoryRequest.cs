using MediatR;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class GetCertificateHistoryRequest : IRequest<PaginatedList<CertificateSummaryResponse>>
    {
        public GetCertificateHistoryRequest()
        {
        }

        public string EndPointAssessorOrganisationId { get; set; }
        public string SearchTerm { get; set; }
        public string SortColumn { get; set; }
        public bool SortDescending { get; set; }
        public int? PageIndex { get; set; }

        public enum SortColumns
        {
            Apprentice,
            Employer,
            ProviderName,
            DateRequested
        }
    }
}