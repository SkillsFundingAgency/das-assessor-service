using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateAddressViewModel : CertificateSendToViewModel
    {
        public string Employer { get; set; }
        
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }

        public bool EditForm { get; set; } = false;

        public bool HasPreviousAddress { get; internal set; }

        public override void FromCertificate(Domain.Entities.Certificate cert)
        {
            base.FromCertificate(cert);

            Employer = CertificateData.ContactOrganisation;
            AddressLine1 = CertificateData.ContactAddLine1;
            AddressLine2 = CertificateData.ContactAddLine2;
            AddressLine3 = CertificateData.ContactAddLine3;
            City = CertificateData.ContactAddLine4;
            Postcode = CertificateData.ContactPostCode;
        }

        public override Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData certData)
        {
            certificate = base.GetCertificateFromViewModel(certificate, certData);

            certData.ContactOrganisation = Employer;
            certData.ContactAddLine1 = AddressLine1;
            certData.ContactAddLine2 = AddressLine2;
            certData.ContactAddLine3 = AddressLine3;
            certData.ContactAddLine4 = City;
            certData.ContactPostCode = Postcode;

            certificate.CertificateData = JsonConvert.SerializeObject(certData);

            return certificate;
        }

        public CertificateAddressViewModel CopyFromCertificateAddress(CertificateAddress certificatePreviousAddress)
        {
            Employer = certificatePreviousAddress.ContactOrganisation;
            AddressLine1 = certificatePreviousAddress.AddressLine1;
            AddressLine2 = certificatePreviousAddress.AddressLine2;
            AddressLine3 = certificatePreviousAddress.AddressLine3;
            City = certificatePreviousAddress.City;
            Postcode = certificatePreviousAddress.PostCode;

            return this;
        }
    }
}