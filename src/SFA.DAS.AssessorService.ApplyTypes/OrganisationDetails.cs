namespace SFA.DAS.AssessorService.ApplyTypes
{
    public class OrganisationDetails
    {
        public string OrganisationReferenceType { get; set; } // "RoEPAO", "RoATP" or "EASAPI"
        public string OrganisationReferenceId { get; set; } // CSV list of known id's

        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public string ProviderName { get; set; }

        public string CompanyNumber { get; set; }
        public string CharityNumber { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }

        public FHADetails FHADetails { get; set; }
    }
}