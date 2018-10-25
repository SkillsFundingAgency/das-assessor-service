using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private
{
    public class CertificateStandardCodeListViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public string SelectedStandardCode { get; set; }
        
        public IEnumerable<SelectListItem> StandardCodes { get; set; }

        public void FromCertificate(Domain.Entities.Certificate cert)
        {
            BaseFromCertificate(cert);
            SelectedStandardCode = cert.StandardCode.ToString();
        }

        public Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData data)
        {
            certificate.StandardCode = Convert.ToInt32(SelectedStandardCode);
            data.StandardLevel = Level;
            data.StandardName = Standard;

            certificate.CertificateData = JsonConvert.SerializeObject(data);
            return certificate;
        }
    }
}