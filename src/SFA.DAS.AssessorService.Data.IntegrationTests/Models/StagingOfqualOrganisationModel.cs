using System;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Models
{
    public class StagingOfqualOrganisationModel : TestModel
    {
        public string RecognitionNumber { get; set; }
        public string Name { get; set; }
        public string LegalName { get; set; }
        public string Acronym { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string HeadOfficeAddressLine1 { get; set; }
        public string HeadOfficeAddressLine2 { get; set; }
        public string HeadOfficeAddressTown { get; set; }
        public string HeadOfficeAddressCounty { get; set; }
        public string HeadOfficeAddressPostcode { get; set; }
        public string HeadOfficeAddressCountry { get; set; }
        public string HeadOfficeAddressTelephone { get; set; }
        public string OfqualStatus { get; set; }
        public DateTime OfqualRecognisedFrom { get; set; }
        public DateTime? OfqualRecognisedTo { get; set; }
    }
}
