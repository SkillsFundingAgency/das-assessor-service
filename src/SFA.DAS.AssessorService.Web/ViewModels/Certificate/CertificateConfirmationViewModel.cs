using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateConfirmationViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public string Reference { get; set; }

        public void FromCertificate(Domain.Entities.Certificate cert)
        {
            BaseFromCertificate(cert);
            Reference = cert.CertificateReference;
        }

        public Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData data)
        {
            certificate.CertificateData = JsonConvert.SerializeObject(data);
            return certificate;
        }
    }
}