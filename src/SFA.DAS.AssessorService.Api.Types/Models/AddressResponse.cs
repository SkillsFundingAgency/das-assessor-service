namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class AddressResponse
    {
        public string Uprn { get; set; }
        public string House { get; set; }
        public string Organisation { get; set; }
        public string Street { get; set; }
        public string Locality { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public double? Match { get; set; }

        public string Text => $"{Organisation}{(string.IsNullOrWhiteSpace(Organisation) ? "" : ", ")}{House} {Street}, {Locality}{(string.IsNullOrWhiteSpace(Locality) ? "" : ", ")}{Town}{(string.IsNullOrWhiteSpace(Town) ? "" : ", ")}{Postcode}";
    }
}
