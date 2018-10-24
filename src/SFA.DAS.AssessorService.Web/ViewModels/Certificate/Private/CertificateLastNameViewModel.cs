using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private
{
    public class CertificateLastNameViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public string LastName { get; set; }

        public void FromCertificate(Domain.Entities.Certificate cert)
        {
            BaseFromCertificate(cert);
            LastName = CertificateData.LearnerFamilyName;
            FullName = CertificateData.FullName;
        }

        public Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData data)
        {
            data.LearnerFamilyName = LastName;
            data.FullName = $"{data.LearnerGivenNames} {data.LearnerFamilyName}";

            certificate.CertificateData = JsonConvert.SerializeObject(data);
            return certificate;
        }
    }
}