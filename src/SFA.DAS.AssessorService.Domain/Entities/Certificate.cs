using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class Certificate : CertificateBase
    {
        public long Uln { get; set; }
        public int StandardCode { get; set; }
        public string StandardUId { get; set; }
        public int ProviderUkPrn { get; set; }
        public Guid OrganisationId {get; set;}

        [JsonIgnore]
        public Organisation Organisation { get; set; }

        public string LearnRefNumber { get; set; }

        public bool IsPrivatelyFunded { get; set; }
        public string PrivatelyFundedStatus { get; set; }
    }
}
