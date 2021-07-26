using System;
using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Domain.JsonData.Printing
{
    public class BatchData
    {
        public int BatchNumber { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? BatchDate { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? PostalContactCount { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] 
        public int? TotalCertificateCount { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? PrintedDate { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? DateOfResponse { get; set; }
    }
}
