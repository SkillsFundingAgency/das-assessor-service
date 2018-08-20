using SFA.DAS.AssessorService.Domain.JsonData;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class StaffBatchSearchResult
    {
        public int? BatchNumber { get; set; }

        public DateTime BatchPrintDate { get; set; }

        public string CertificateReference { get; set; }

        public string Status { get; set; }

        public CertificateData CertificateData { get; set; }


        // The below are things which may be useful
        public long Uln { get; set; }
        public int StandardCode { get; set; }             
    }
}
