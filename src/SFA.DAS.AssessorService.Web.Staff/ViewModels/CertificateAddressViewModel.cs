using System;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels
{
    public class CertificateAddressViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public string Employer { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }



        public void FromCertificate(Certificate cert)
        {
            BaseFromCertificate(cert);
            Employer = CertificateData.ContactOrganisation;
            AddressLine1 = CertificateData.ContactAddLine1;
            AddressLine2 = CertificateData.ContactAddLine2;
            AddressLine3 = CertificateData.ContactAddLine3;
            City = CertificateData.ContactAddLine4;
            Postcode = CertificateData.ContactPostCode;
        }
        
        public AssessorService.Domain.Entities.Certificate GetCertificateFromViewModel(AssessorService.Domain.Entities.Certificate certificate, CertificateData data)
        {
            if (certificate == null) throw new ArgumentNullException(nameof(certificate));
            if (certificate == null) throw new ArgumentNullException(nameof(certificate));
            data.ContactOrganisation = Employer;
            data.ContactAddLine1 = AddressLine1;
            data.ContactAddLine2 = AddressLine2;
            data.ContactAddLine3 = AddressLine3;
            data.ContactAddLine4 = City;
            data.ContactPostCode = Postcode;

            certificate.CertificateData = JsonConvert.SerializeObject(data);

            return certificate;
        }

        //public CertificateAddressViewModel CopyFromCertificateAddress(CertificateAddress certificatePreviousAddress)
        //{
        //    this.AddressLine1 = certificatePreviousAddress.AddressLine1;
        //    this.AddressLine2 = certificatePreviousAddress.AddressLine2;
        //    this.AddressLine3 = certificatePreviousAddress.AddressLine3;
        //    this.City = certificatePreviousAddress.City;
        //    this.Postcode = certificatePreviousAddress.PostCode;

        //    return this;
        //}
    }
}