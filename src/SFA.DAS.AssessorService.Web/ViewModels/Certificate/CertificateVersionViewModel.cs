using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;
using System.Collections.Generic;
using SFA.DAS.AssessorService.Web.ViewModels.Shared;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using System.Linq;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateVersionViewModel : CertificateBaseViewModel
    {
        public IEnumerable<StandardVersionViewModel> Versions { get; set; }

        public void FromCertificate(Domain.Entities.Certificate cert, IEnumerable<StandardVersionViewModel> versions)
        {
            base.FromCertificate(cert);
            Versions = versions;
        }

        public Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, StandardVersion standardVersion, StandardOptions options)
        {
            var certData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
            certData.StandardReference = standardVersion.IFateReferenceNumber;
            certData.StandardName = standardVersion.Title;
            certData.StandardLevel = standardVersion.Level;
            certData.StandardPublicationDate = standardVersion.EffectiveFrom;
            certData.Version = standardVersion.Version.ToString();

            if (options != null && options.OnlyOneOption())
            {
                // If only one option set on the certificate
                certData.CourseOption = options.CourseOption.Single();
            }
            else if(certificate.StandardUId != StandardUId)
            {
                // If changed, wipe the option in case different versions have different options
                certData.CourseOption = null;
            }

            certificate.CertificateData = JsonConvert.SerializeObject(certData);
            certificate.StandardUId = StandardUId;
            return certificate;
        }
    }
}