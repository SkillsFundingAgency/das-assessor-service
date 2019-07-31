using System;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private
{
    public class CertificateUlnViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public AssessorService.Domain.Entities.Certificate GetCertificateFromViewModel(AssessorService.Domain.Entities.Certificate certificate, CertificateData data)
        {
            certificate.Uln = Convert.ToInt64(Uln);

            certificate.CertificateData = JsonConvert.SerializeObject(data);
            return certificate;
        }
    }
}