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
            if (SendToHasChanged(certData))
            {
                certData = ClearFieldsAfterSendToSwitch(certData);
            }

            if(SendTo == CertificateSendTo.Apprentice)
            {
                // when sending to the apprentice use the apprentice name for the contact
                certData.ContactName = certData.FullName;
            }

            certData.SendTo = SendTo;
            certificate.CertificateData = JsonConvert.SerializeObject(certData);
            
            return certificate;
        }

        public bool SendToHasChanged(CertificateData certData)
        {
            return certData.SendTo != SendTo;
        }

        private CertificateData ClearFieldsAfterSendToSwitch(CertificateData certData)
        {
            certData.Department = string.Empty;
            certData.ContactName = string.Empty;
            certData.ContactOrganisation = string.Empty;
            certData.ContactAddLine1 = string.Empty;
            certData.ContactAddLine2 = string.Empty;
            certData.ContactAddLine3 = string.Empty;
            certData.ContactAddLine4 = string.Empty;
            certData.ContactPostCode = string.Empty;

            return certData;
        }
    }
}