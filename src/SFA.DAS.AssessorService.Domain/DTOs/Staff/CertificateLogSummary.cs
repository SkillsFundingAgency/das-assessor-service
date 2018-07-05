using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Domain.DTOs.Staff
{
    public class CertificateLogSummary
    {
        public DateTime EventTime { get; set; }
        public string Action { get; set; }
        public string ActionBy { get; set; }
        public string Status { get; set; }
        public string CertificateData { get; set; }
        public Dictionary<string, string> DifferencesToPrevious { get; set; }
    }
}