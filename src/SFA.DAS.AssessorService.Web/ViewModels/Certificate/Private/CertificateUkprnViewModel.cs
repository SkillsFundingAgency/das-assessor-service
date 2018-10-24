using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private
{
    public class CertificateUkprnViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public string Ukprn { get; set; }

        public void FromCertificate(Domain.Entities.Certificate cert)
        {
            BaseFromCertificate(cert);
            Ukprn = cert.ProviderUkPrn != 0 ? 
                cert.ProviderUkPrn.ToString() : string.Empty;
        }

        public Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData data)
        {
            certificate.ProviderUkPrn = Convert.ToInt32(Ukprn);

            certificate.CertificateData = JsonConvert.SerializeObject(data);
            return certificate;
        }
    }
}