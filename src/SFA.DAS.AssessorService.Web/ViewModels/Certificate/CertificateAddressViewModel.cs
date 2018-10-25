﻿using System;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateAddressViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public string Employer { get; set; }
        public string Name { get; set; }
        public string Dept { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }

        public bool SelectPreviousAddress { get; set; }

        public bool EditForm { get; set; } = false;
        public bool ResetForm { get; set; } = false;        

        public CertificatePreviousAddressViewModel PreviousAddress { get; set; }

        public void FromCertificate(Domain.Entities.Certificate cert)
        {
            BaseFromCertificate(cert);

            Employer = CertificateData.ContactOrganisation;
            Name = CertificateData.ContactName;
            Dept = CertificateData.Department;

            AddressLine1 = CertificateData.ContactAddLine1;
            AddressLine2 = CertificateData.ContactAddLine2;
            AddressLine3 = CertificateData.ContactAddLine3;
            City = CertificateData.ContactAddLine4;
            Postcode = CertificateData.ContactPostCode;
        }

        public void EmptyAddressDetails()
        {
            this.Employer = String.Empty;
            this.AddressLine1 = String.Empty;
            this.AddressLine2 = String.Empty;
            this.AddressLine3 = String.Empty;
            this.City = String.Empty;
            this.Postcode = String.Empty;
        }

        public Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData data)
        {
            data.ContactOrganisation = Employer;

            data.ContactAddLine1 = AddressLine1;
            data.ContactAddLine2 = AddressLine2;
            data.ContactAddLine3 = AddressLine3;
            data.ContactAddLine4 = City;
            data.ContactPostCode = Postcode;

            certificate.CertificateData = JsonConvert.SerializeObject(data);

            return certificate;
        }

        public CertificateAddressViewModel CopyFromCertificateAddress(CertificateAddress certificatePreviousAddress)
        {
            this.Employer = certificatePreviousAddress.ContactOrganisation;
           
            this.AddressLine1 = certificatePreviousAddress.AddressLine1;
            this.AddressLine2 = certificatePreviousAddress.AddressLine2;
            this.AddressLine3 = certificatePreviousAddress.AddressLine3;
            this.City = certificatePreviousAddress.City;
            this.Postcode = certificatePreviousAddress.PostCode;

            return this;
        }
    }
}