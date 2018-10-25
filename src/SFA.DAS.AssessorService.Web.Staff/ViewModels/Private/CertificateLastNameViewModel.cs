using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Private
{
    public class CertificateLastNameViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public string LastName { get; set; }

        public void FromCertificate(AssessorService.Domain.Entities.Certificate cert)
        {
            BaseFromCertificate(cert);
            LastName = CertificateData.LearnerFamilyName;
            FullName = CertificateData.FullName;
        }

        public AssessorService.Domain.Entities.Certificate GetCertificateFromViewModel(AssessorService.Domain.Entities.Certificate certificate, CertificateData data)
        {
            data.LearnerFamilyName = LastName;
            data.FullName = $"{data.LearnerGivenNames} {data.LearnerFamilyName}";

            certificate.CertificateData = JsonConvert.SerializeObject(data);
            return certificate;
        }
    }
}