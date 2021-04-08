using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateVersionViewModel : CertificateBaseViewModel
    {        
        public IEnumerable<StandardVersion> Versions { get; set; }

        public class StandardVersion
        {
            public string Title { get; set; }
            public string StandardUId { get; set; }
            public string Version { get; set; }

            public static implicit operator StandardVersion(Api.Types.Models.Standards.StandardVersion version)
            {
                return new StandardVersion
                {
                    StandardUId = version.StandardUId,
                    Title = version.Title,
                    Version = version.Version
                };
            }
        }

        public void FromCertificate(Domain.Entities.Certificate cert, IEnumerable<StandardVersion> versions)
        {
            base.FromCertificate(cert);
            Versions = versions;
        }

        public Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData data)
        {
            if(certificate.StandardUId != StandardUId)
            {
                var certData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
                certData.CourseOption = null;
                certificate.CertificateData = JsonConvert.SerializeObject(certData);
            }

            certificate.StandardUId = StandardUId;           
            return certificate;
        }
    }
}