using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels
{
    public class CertificateApprenticeDetailsViewModel : CertificateBaseViewModel, ICertificateViewModel
    {        
        public void FromCertificate(Certificate cert)
        {
            BaseFromCertificate(cert);
            FamilyName = CertificateData.LearnerFamilyName;
        }

        public Certificate GetCertificateFromViewModel(Certificate certificate, CertificateData data)
        {
            data.LearnerFamilyName = FamilyName;
            data.LearnerGivenNames = GivenNames;

            certificate.CertificateData = JsonConvert.SerializeObject(data);

            return certificate;
        }
    }
}