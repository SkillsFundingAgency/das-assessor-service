using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateRecipientViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public string Name { get; set; }
        public string Dept { get; set; }

        public void FromCertificate(Domain.Entities.Certificate cert)
        {
            BaseFromCertificate(cert);
            Name = CertificateData.ContactName;
            Dept = CertificateData.Department;
        }

        public Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData data)
        {
            data.ContactName = Name;
            data.Department = Dept;
           
            certificate.CertificateData = JsonConvert.SerializeObject(data);

            return certificate;
        }
    }
}