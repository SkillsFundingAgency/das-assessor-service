using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels
{
    public class CertificateOptionViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public bool? HasAdditionalLearningOption { get; set; }
        public string Option { get; set; }
        public void FromCertificate(Domain.Entities.Certificate cert)
        {
            BaseFromCertificate(cert);
            Option = CertificateData.CourseOption;

            if (CertificateData.CourseOption == null)
            {
                HasAdditionalLearningOption = null;
            }
            else
            {
                HasAdditionalLearningOption = CertificateData.CourseOption != "";
            }
        }

        public Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData data)
        {
            data.CourseOption = HasAdditionalLearningOption.Value ? Option : "";
            certificate.CertificateData = JsonConvert.SerializeObject(data);

            return certificate;
        }
    }
}