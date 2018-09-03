using System;
using System.Collections.Generic;
using System.Text;

namespace TestConsoleApp.Models
{
    public class CertificateAddress
    {
        public Guid OrganisationId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
