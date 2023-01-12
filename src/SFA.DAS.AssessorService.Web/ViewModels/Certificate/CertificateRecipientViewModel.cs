using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateRecipientViewModel : CertificateAddressViewModel
    {
        public string Name { get; set; }
        public string Dept { get; set; }

        public override void FromCertificate(Domain.Entities.Certificate cert)
        {
            base.FromCertificate(cert);

            Name = CertificateData.ContactName;
            Dept = CertificateData.Department;
        }

        public override Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData certData)
        {
            certificate = base.GetCertificateFromViewModel(certificate, certData);

            certData.ContactName = Name;
            certData.Department = Dept;

            certificate.CertificateData = JsonConvert.SerializeObject(certData);

            return certificate;
        }

        public bool RecipientHasChanged(CertificateData certData)
        {
            return
                certData.ContactName != Name ||
                certData.Department != Dept;
        }
    }
}