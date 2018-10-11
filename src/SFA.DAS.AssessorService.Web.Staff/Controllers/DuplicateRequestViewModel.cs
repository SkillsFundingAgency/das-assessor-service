using System;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    public class DuplicateRequestViewModel
    {
        public Guid CertificateId { get; set; }
        public bool IsConfirmed { get; set; }
        public string NextBatchDate { get; set; }
        public string SearchString { get; set; }
        public int Page { get; set; }
        public long Uln { get; set; }
        public int StdCode { get; set; }
        public bool BackToCheckPage { get; set; }
        public string CertificateReference { get; set; }
        public string Status { get; set; }
    }
}