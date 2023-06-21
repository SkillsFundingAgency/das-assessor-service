using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateNamesViewModel : CertificateBaseViewModel
    {
        public string InputGivenNames { get; set; }
        public string InputFamilyName { get; set; }

        public override Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData certData)
        {
            certData.LearnerGivenNames = GivenNames;
            certData.LearnerFamilyName = FamilyName;
            certData.FullName = GivenNames + FamilyName;

            certificate.CertificateData = JsonConvert.SerializeObject(certData);

            return certificate;
        }
    }
}
