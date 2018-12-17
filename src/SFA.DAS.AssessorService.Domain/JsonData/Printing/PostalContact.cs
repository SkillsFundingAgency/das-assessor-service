namespace SFA.DAS.AssessorService.Api.Types.Models.Printing
{
    public class PostalContact
    {
        public string Name { get; set; }
        public string Department { get; set; }
        public string EmployerName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string Postcode { get; set; }
        public int CertificateCount { get; set; }
    }
}
