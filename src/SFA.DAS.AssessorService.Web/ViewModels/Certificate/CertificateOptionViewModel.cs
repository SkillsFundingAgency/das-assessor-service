using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
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

        public CertificateData GetCertificateDataFromViewModel(CertificateData data)
        {
            data.CourseOption = HasAdditionalLearningOption.Value ? Option : "";
            return data;
        }
    }
}