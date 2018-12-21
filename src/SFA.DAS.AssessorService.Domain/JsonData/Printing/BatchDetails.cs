using System;
using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Domain.JsonData.Printing
{
    public class BatchDetails
    {
        public int BatchNumber { get; set; }
        public DateTime BatchDate { get; set; }
        public int PostalContactCount { get; set; }
        public int TotalCertificateCount { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? PrintedDate { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? PostedDate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? DateOfResponse { get; set; }
    }
}
