using System.Collections.Generic;
using System.Linq;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Private
{
    public class CertificateApprovalViewModel
    {     
        public PaginatedList<CertificateDetailApprovalViewModel> ApprovedCertificates { get; set; }
        public PaginatedList<CertificateDetailApprovalViewModel> RejectedCertificates { get; set; }
        public PaginatedList<CertificateDetailApprovalViewModel> ToBeApprovedCertificates { get; set; }
        public PaginatedList<CertificateDetailApprovalViewModel> SentForApprovalCertificates { get; set; }


        public CertificateDetailApprovalViewModel[] ApproveResults()
        {
            var count = ApprovedCertificates.Items.Count();
            var approvalResults =
                new CertificateDetailApprovalViewModel[count];
            for (var i = 0; i < count; i++)
            {
                approvalResults[i] = new CertificateDetailApprovalViewModel();
            }

            return approvalResults;
        }

        public CertificateDetailApprovalViewModel[] ToBeApproveResults()
        {
            var count = ToBeApprovedCertificates.Items.Count();
            var approvalResults =
                new CertificateDetailApprovalViewModel[count];
            for (var i = 0; i < count; i++)
            {
                approvalResults[i] = new CertificateDetailApprovalViewModel();
            }

            return approvalResults;
        }

        public CertificateDetailApprovalViewModel[] RejectedResults()
        {
            var count = RejectedCertificates.Items.Count();
            var approvalResults =
                new CertificateDetailApprovalViewModel[count];
            for (var i = 0; i < count; i++)
            {
                approvalResults[i] = new CertificateDetailApprovalViewModel();
            }

            return approvalResults;
        }

        public CertificateDetailApprovalViewModel[] SentForApprovalsResults()
        {
            var count = SentForApprovalCertificates?.Items?.Count() ?? ToBeApprovedCertificates.Items.Count;
            var approvalResults =
                new CertificateDetailApprovalViewModel[count];
            for (var i = 0; i < count; i++)
            {
                approvalResults[i] = new CertificateDetailApprovalViewModel();
            }

            return approvalResults;
        }
    }
}