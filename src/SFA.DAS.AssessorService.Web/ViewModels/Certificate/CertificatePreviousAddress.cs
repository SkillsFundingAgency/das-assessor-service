using SFA.DAS.AssessorService.Web.Extensions;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificatePreviousAddressViewModel
    {
        public string ContactOrganisation { get; set; }
        public string AddressLine1 { get; }
        public string AddressLine2 { get; }
        public string AddressLine3 { get; }
        public string City { get; }
        public string Postcode { get; }

        public string StringifiedAddress { get; set; }

        public long HashCode { get; set; }

        public CertificatePreviousAddressViewModel(Domain.Entities.CertificateAddress certificateAddress)
        {
            ContactOrganisation = certificateAddress.ContactOrganisation;
            AddressLine1 = certificateAddress.AddressLine1;
            AddressLine2 = certificateAddress.AddressLine2;
            AddressLine3 = certificateAddress.AddressLine3;
            City = certificateAddress.City;
            Postcode = certificateAddress.PostCode;

            StringifiedAddress = StringifyAddress();

            HashCode = StringifiedAddress.GetInt64HashCode();
        }

        private string StringifyAddress()
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(this.ContactOrganisation))
            {
                result += (this.ContactOrganisation);
            }

            if (!string.IsNullOrEmpty(this.AddressLine1))
            {
                result += $", {this.AddressLine1}";
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

            if (!string.IsNullOrEmpty(this.Postcode))
            {
                result += $", {this.Postcode}";
            }

            return result;
        }
    }
}
