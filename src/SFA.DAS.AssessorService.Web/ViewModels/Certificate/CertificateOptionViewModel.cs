using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateOptionViewModel : CertificateBaseViewModel
    {
        public CertificateOptionViewModel()
        {
            
        }
        public CertificateOptionViewModel(Domain.Entities.Certificate certificate) : base(certificate)
        {
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

        public bool? HasAdditionalLearningOption { get; set; }
        public string Option { get; set; }
    }
}