using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Infrastructure.ApiClients.OuterApi
{
    public class GetAddressesResponse
    {
        public GetAddressesResponse()
        {
            Addresses = new List<GetAddressResponse>();
        }

        [JsonProperty("addresses")]
        public IEnumerable<GetAddressResponse> Addresses { get; set; }
    }

    public class GetAddressResponse
    {
        [JsonProperty("uprn")]
        public string Uprn { get; set; }

        [JsonProperty("premises")]
        public string Premises { get; set; }

        [JsonProperty("thoroughfare")]
        public string Thoroughfare { get; set; }

        [JsonProperty("organisation")]
        public string Organisation { get; set; }

        [JsonProperty("locality")]
        public string Locality { get; set; }

        [JsonProperty("posttown")]
        public string Town { get; set; }

        [JsonProperty("county")]
        public string County { get; set; }

        [JsonProperty("postcode")]
        public string Postcode { get; set; }

        [JsonProperty("addressline1")]
        public string AddressLine1 { get; set; }

        [JsonProperty("addressline2")]
        public string AddressLine2 { get; set; }

        [JsonProperty("addressline3")]
        public string AddressLine3 { get; set; }

        [JsonProperty("longitude")]
        public double? Longitude { get; set; }

        [JsonProperty("latitude")]
        public double? Latitude { get; set; }

        [JsonProperty("match")]
        public double? Match { get; set; }
    }
}
