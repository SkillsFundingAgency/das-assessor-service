using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private
{
    public class CertificateUkprnListViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public int SelectedUkprn { get; set; }
        public IEnumerable<SelectListItem> Ukprns { get; set; }

        public void FromCertificate(Domain.Entities.Certificate cert)
        {
            BaseFromCertificate(cert);
            SelectedUkprn = cert.ProviderUkPrn;
        }

        public Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData data)
        {
            certificate.ProviderUkPrn = SelectedUkprn;

            certificate.CertificateData = JsonConvert.SerializeObject(data);
            return certificate;
        }
    }
}