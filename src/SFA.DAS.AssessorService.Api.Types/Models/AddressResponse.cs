namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class AddressResponse
    {
        public string Uprn { get; set; }
        public string Organisation { get; set; }
        public string Premises { get; set; }
        public string Thoroughfare { get; set; }
        public string Locality { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public double? Match { get; set; }

        public string Text => 
            $"{Organisation}{(string.IsNullOrWhiteSpace(Organisation) ? "" : ", ")}" +
            $"{AddressLine1}, " +
            $"{AddressLine2}{(string.IsNullOrWhiteSpace(AddressLine2) ? "" : ", ")}" +
            $"{AddressLine3}{(string.IsNullOrWhiteSpace(AddressLine3) ? "" : ", ")}" +
            $"{Town}{(string.IsNullOrWhiteSpace(Town) ? "" : ", ")}" +
            $"{Postcode}";
    }
}
