using System.Collections.Generic;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateOptionViewModel : CertificateBaseViewModel
    {
        
        public string Option { get; set; }
        public List<string> Options { get; set; }

        public void FromCertificate(Domain.Entities.Certificate cert, List<string> options)
        {
            BaseFromCertificate(cert);
            Option = CertificateData.CourseOption;
            Options = options;
        }

        public Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData data)
        {
            data.CourseOption = Option;
            certificate.CertificateData = JsonConvert.SerializeObject(data);

            return certificate;
        }
    }
}