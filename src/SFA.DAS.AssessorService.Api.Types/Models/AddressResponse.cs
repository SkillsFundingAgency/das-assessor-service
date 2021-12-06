using System.Linq;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class AddressResponse
    {
        private const string NoResultsFoundText = "No results found";

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

        public string Text
        {
            get
            {
                string[] addressParts = new string[] { Organisation, AddressLine1, AddressLine2, AddressLine3, Town, Postcode };
                return string.Join(", ", addressParts.Where(p => !string.IsNullOrWhiteSpace(p)));
            }
        }

        public static AddressResponse NoResultsFound => new AddressResponse { Town = NoResultsFoundText };
    }
}
