using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class CertificateResponse
    {
        public long Uln { get; set; }
        public int StandardCode { get; set; }
        public int ProviderUkPrn { get; set; }     

        public string EndPointAssessorOrganisationId { get; set; }
        public string EndPointAssessorOrganisationName { get; set; }

        public string CertificateReference { get; set; }

        public string BatchNumber { get; set; }

        public CertificateDataResponse CertificateData { get; set; }

        public string Status { get; set; }
    }
}
