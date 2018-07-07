using System;

namespace SFA.DAS.AssessorService.Domain.Entities
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

        public string StringifyAddress()
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(this.AddressLine1))
            {
                result += (this.AddressLine1);
            }

            if (!string.IsNullOrEmpty(this.AddressLine2))
            {
                result += $", {this.AddressLine2}";
            }

            if (!string.IsNullOrEmpty(this.AddressLine3))
            {
                result += $", {this.AddressLine3}";
            }

            if (!string.IsNullOrEmpty(this.City))
            {
                result += $", {this.City}";
            }

            if (!string.IsNullOrEmpty(this.PostCode))
            {
                result += $", {this.PostCode}";
            }

            return result;
        }
    }
}
