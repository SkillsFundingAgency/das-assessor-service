using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels
{
    public class CertificateApprenticeDetailsViewModel : CertificateBaseViewModel, ICertificateViewModel
    {        
        public void FromCertificate(Domain.Entities.Certificate cert)
        {
            BaseFromCertificate(cert);
            FamilyName = CertificateData.LearnerFamilyName;
        }

        public Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData data)
        {
            data.LearnerFamilyName = FamilyName;
            certificate.CertificateData = JsonConvert.SerializeObject(data);

            return certificate;
        }
    }
}