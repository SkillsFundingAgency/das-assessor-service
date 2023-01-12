using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateDeclarationViewModel : CertificateBaseViewModel
    {
        public override Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData certData)
        {
            certificate.CertificateData = JsonConvert.SerializeObject(certData);
            return certificate;
        }
    }
}