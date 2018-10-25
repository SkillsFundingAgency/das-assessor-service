using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private
{
    public class CertificateFirstNameViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public string FirstName { get; set; }

        public void FromCertificate(Domain.Entities.Certificate cert)
        {
            BaseFromCertificate(cert);
            FirstName = CertificateData.LearnerGivenNames;
            FullName = CertificateData.FullName;
        }

        public Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData data)
        {
            data.LearnerGivenNames = FirstName;
            data.FullName = $"{data.LearnerGivenNames} {data.LearnerFamilyName}";

            certificate.CertificateData = JsonConvert.SerializeObject(data);
            return certificate;
        }
    }
}