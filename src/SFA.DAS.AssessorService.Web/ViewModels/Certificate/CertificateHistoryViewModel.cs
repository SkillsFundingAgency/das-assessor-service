using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Paging;
using System.Linq;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateHistoryViewModel
    {
        public int? PageIndex { get; set; }

        public GetCertificateHistoryRequest.SortColumns SortColumn { get; set; }
        public string SortDirection { get; set; }
        public string SearchTerm { get; set; }

        public PaginatedList<CertificateSummaryResponse> Certificates { get; set; }

        public bool CanShowCertificateDetails(CertificateSummaryResponse certificateSummaryResponse)
        {
            var allowed = new[]
            {
                CertificateStatus.Submitted, CertificateStatus.SentToPrinter,
                CertificateStatus.Printed, CertificateStatus.Reprint,
                CertificateStatus.Delivered, CertificateStatus.NotDelivered,
                CertificateStatus.ToBeApproved
            };

            return
                allowed.Contains(certificateSummaryResponse.Status) &&
                certificateSummaryResponse.OverallGrade != CertificateGrade.Fail;
        }

        public bool IsCertificateWaitingToBeDelivered(CertificateSummaryResponse certificateSummaryResponse)
        {
            var allowed = new[]
            {
                CertificateStatus.Submitted, CertificateStatus.SentToPrinter,
                CertificateStatus.Printed, CertificateStatus.Reprint
            };

            return
                allowed.Contains(certificateSummaryResponse.Status);
        }

        public bool IsCertificateDelivered(CertificateSummaryResponse certificateSummaryResponse)
        {
            return certificateSummaryResponse.Status == CertificateStatus.Delivered;
        }

        public bool IsCertificateNotDelivered(CertificateSummaryResponse certificateSummaryResponse)
        {
            return certificateSummaryResponse.Status == CertificateStatus.NotDelivered;
        }
    }
}
