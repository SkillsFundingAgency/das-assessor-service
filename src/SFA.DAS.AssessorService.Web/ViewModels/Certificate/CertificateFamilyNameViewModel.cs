using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateFamilyNameViewModel : CertificateBaseViewModel
    {
        public override Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData certData)
        {
            certData.LearnerFamilyName = FamilyName;
            certData.FullName = GivenNames + FamilyName;

            certificate.CertificateData = certData;

            return certificate;
        }
    }
}
