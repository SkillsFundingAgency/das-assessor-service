using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    using Certificate = Domain.Entities.Certificate;

    public class CertificateSendToViewModel : CertificateBaseViewModel
    {
        public CertificateSendTo SendTo { get; set; }

        public override void FromCertificate(Certificate certificate)
        {
            base.FromCertificate(certificate);

            SendTo = CertificateData.SendTo;
        }

        public override Certificate GetCertificateFromViewModel(Certificate certificate, CertificateData certData)
        {
            certData.SendTo = SendTo;
            certificate.CertificateData = JsonConvert.SerializeObject(certData);

            return certificate;
        }

        public bool SendToHasChanged(CertificateData certData)
        {
            return certData.SendTo != SendTo;
        }
    }
}