using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateFirstNameViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public string FirstName { get; set; }
        
        public void FromCertificate(Domain.Entities.Certificate cert)
        {
            BaseFromCertificate(cert);
            FirstName = CertificateData.LearnerGivenNames;
        }

        public Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData data)
        {
            data.LearnerGivenNames = FirstName;

            certificate.CertificateData = JsonConvert.SerializeObject(data);
            return certificate;
        }
    }
}