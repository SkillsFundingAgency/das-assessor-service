using System;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Private
{
    public class CertificateUkprnViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public string Ukprn { get; set; }

        public void FromCertificate(AssessorService.Domain.Entities.Certificate cert)
        {
            BaseFromCertificate(cert);
            Ukprn = cert.ProviderUkPrn != 0 ? 
                cert.ProviderUkPrn.ToString() : string.Empty;
        }

        public AssessorService.Domain.Entities.Certificate GetCertificateFromViewModel(AssessorService.Domain.Entities.Certificate certificate, CertificateData data)
        {
            certificate.ProviderUkPrn = Convert.ToInt32(Ukprn);

            certificate.CertificateData = JsonConvert.SerializeObject(data);
            return certificate;
        }
    }
}