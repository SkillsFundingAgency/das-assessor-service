using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateConfirmationViewModel : CertificateBaseViewModel
    {
        public string SelectedGrade { get; set; }
        public string Reference { get; set; }

        public override void FromCertificate(Domain.Entities.Certificate cert)
        {
            base.FromCertificate(cert);

            Reference = cert.CertificateReference;
            SelectedGrade = CertificateData.OverallGrade;
            GivenNames = CertificateData.LearnerGivenNames;
            FamilyName = CertificateData.LearnerFamilyName;
        }

        public override Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData certData)
        {
            certificate.CertificateData = JsonConvert.SerializeObject(certData);
            return certificate;
        }
    }
}