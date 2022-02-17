using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Domain.DTOs.Staff
{
    public class CertificateLogSummary
    {
        public DateTime EventTime { get; set; }
        public string Action { get; set; }
        public string ActionBy { get; set; }
        public string ActionByEmail { get; set; }
        public string Status { get; set; }
        public string CertificateData { get; set; }
        public List<Difference> DifferencesToPrevious { get; set; }
        public int? BatchNumber { get; set; }
        public string ReasonForChange { get; set; }

        public class Difference
        {
            public string Key { get; set; }
            public List<string> Values { get; set; }
            public bool IsList { get; set; } = false;
        }
    }
}