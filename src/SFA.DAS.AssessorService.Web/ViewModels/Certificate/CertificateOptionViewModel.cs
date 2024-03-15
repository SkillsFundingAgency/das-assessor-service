using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateOptionViewModel : CertificateBaseViewModel
    {
        
        public string Option { get; set; }
        public List<string> Options { get; set; }
        public void FromCertificate(Domain.Entities.Certificate cert, List<string> options)
        {
            base.FromCertificate(cert);

            Option = CertificateData.CourseOption;
            Options = options.OrderBy(s => s).ToList();
        }

        public override Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData certData)
        {
            certData.CourseOption = Option;
            certificate.CertificateData = JsonConvert.SerializeObject(certData);

            return certificate;
        }
    }
}