using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Private
{
    public class CertificateStandardCodeListViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public string SelectedStandardCode { get; set; }
        
        public IEnumerable<SelectListItem> StandardCodes { get; set; }

        public void FromCertificate(AssessorService.Domain.Entities.Certificate cert)
        {
            BaseFromCertificate(cert);
            SelectedStandardCode = cert.StandardCode.ToString();
        }

        public AssessorService.Domain.Entities.Certificate GetCertificateFromViewModel(AssessorService.Domain.Entities.Certificate certificate, CertificateData data)
        {
            certificate.StandardCode = Convert.ToInt32(SelectedStandardCode);
            data.StandardLevel = Level;
            data.StandardName = Standard;

            certificate.CertificateData = JsonConvert.SerializeObject(data);
            return certificate;
        }
    }
}