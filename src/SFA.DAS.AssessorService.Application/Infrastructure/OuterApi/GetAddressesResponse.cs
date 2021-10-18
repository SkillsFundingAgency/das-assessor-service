using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Application.Infrastructure.OuterApi
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

        [JsonProperty("house")]
        public string House { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

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

        [JsonProperty("longitude")]
        public double? Longitude { get; set; }

        [JsonProperty("latitude")]
        public double? Latitude { get; set; }

        [JsonProperty("match")]
        public double? Match { get; set; }
    }
}
