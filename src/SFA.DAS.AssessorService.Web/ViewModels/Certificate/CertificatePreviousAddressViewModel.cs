using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificatePreviousAddressViewModel : CertificateSendToViewModel
    {
        public long? EmployerAccountId { get; set; }
        public string EmployerName { get; set; }
        public bool HasPreviousAddress { get; set; }
        public bool? UsePreviousAddress { get; set; }
        public CertificateAddress PreviousAddress { get; set; }

        public override void FromCertificate(Domain.Entities.Certificate cert)
        {
            base.FromCertificate(cert);

            EmployerAccountId = CertificateData.EmployerAccountId;
        }

        public override Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData certData)
        {
            certificate = base.GetCertificateFromViewModel(certificate, certData);

            if(UsePreviousAddress.Value)
            {
                certData.ContactOrganisation = PreviousAddress.ContactOrganisation;
                certData.ContactAddLine1 = PreviousAddress.AddressLine1;
                certData.ContactAddLine2 = PreviousAddress.AddressLine2;
                certData.ContactAddLine3 = PreviousAddress.AddressLine3;
                certData.ContactAddLine4 = PreviousAddress.City;
                certData.ContactPostCode = PreviousAddress.PostCode;
            }
            else
            {
                certData.ContactOrganisation = string.Empty;
                certData.ContactAddLine1 = string.Empty;
                certData.ContactAddLine2 = string.Empty;
                certData.ContactAddLine3 = string.Empty;
                certData.ContactAddLine4 = string.Empty;
                certData.ContactPostCode = string.Empty;
            }
            
            certificate.CertificateData = certData;

            return certificate;
        }
    }
}