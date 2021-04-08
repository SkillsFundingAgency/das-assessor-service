using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateVersionViewModel : CertificateBaseViewModel
    {        
        public Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData data)
        {
            certificate.StandardUId = StandardUId;           
            return certificate;
        }
    }
}