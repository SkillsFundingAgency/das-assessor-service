using System;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Private
{
    public class CertificateUlnViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public string Uln { get; set; }

        public void FromCertificate(AssessorService.Domain.Entities.Certificate cert)
        {
            BaseFromCertificate(cert);
            Uln = cert.Uln.ToString();
        }

        public AssessorService.Domain.Entities.Certificate GetCertificateFromViewModel(AssessorService.Domain.Entities.Certificate certificate, CertificateData data)
        {
            certificate.Uln = Convert.ToInt64(Uln);

            certificate.CertificateData = JsonConvert.SerializeObject(data);
            return certificate;
        }
    }
}