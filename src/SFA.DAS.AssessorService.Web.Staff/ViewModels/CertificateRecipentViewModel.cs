using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels
{
    public class CertificateRecipientViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public string Name { get; set; }
        public string Dept { get; set; }

        public void FromCertificate(Certificate cert)
        {
            BaseFromCertificate(cert);
            Name = CertificateData.ContactName;
            Dept = CertificateData.Department;
        }

        public Certificate GetCertificateFromViewModel(Certificate certificate, CertificateData data)
        {
            data.ContactName = Name;
            data.Department = Dept;

            certificate.CertificateData = JsonConvert.SerializeObject(data);

            return certificate;
        }
    }
}