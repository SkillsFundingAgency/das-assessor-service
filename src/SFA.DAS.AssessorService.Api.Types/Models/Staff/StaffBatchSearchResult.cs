using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace SFA.DAS.AssessorService.Api.Types.Models.Staff
{
    public class StaffBatchSearchResult
    {
        public int? BatchNumber { get; set; }
        public DateTime StatusAt { get; set; }
        public string CertificateReference { get; set; }
        public string Status { get; set; }
        public CertificateData CertificateData { get; set; }
        public long Uln { get; set; }
        public int StandardCode { get; set; }
        public Guid? FrameworkLearnerId { get; set; }
    }
}
