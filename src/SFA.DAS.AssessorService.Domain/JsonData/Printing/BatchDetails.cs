using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Printing
{
    public class BatchDetails
    {
        public int BatchNumber { get; set; }
        public DateTime BatchDate { get; set; }
        public int PostalContactCount { get; set; }
        public int TotalCertificateCount { get; set; }
        //public DateTime? PrintedDate { get; set; }
        //public DateTime? PostedDate { get; set; }
    }
}
