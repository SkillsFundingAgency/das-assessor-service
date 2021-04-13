using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;
using System.Collections.Generic;
using SFA.DAS.AssessorService.Web.ViewModels.Shared;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateVersionViewModel : CertificateBaseViewModel
    {
        public IEnumerable<StandardVersion> Versions { get; set; }

        public void FromCertificate(Domain.Entities.Certificate cert, IEnumerable<StandardVersion> versions)
        {
            base.FromCertificate(cert);
            Versions = versions;
        }

        public Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, bool standardVersionChanged, Api.Types.Models.Standards.StandardVersion standardVersion)
        {

            var certData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
            certData.StandardReference = standardVersion.IFateReferenceNumber;
            certData.StandardName = standardVersion.Title;
            certData.StandardLevel = standardVersion.Level;
            certData.StandardPublicationDate = standardVersion.EffectiveFrom;
            certData.Version = standardVersion.Version.ToString();

            if (standardVersionChanged)
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