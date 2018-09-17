using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels
{
    public class CertificateGradeViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public string SelectedGrade { get; set; }
        public List<SelectListItem> Grades = new List<SelectListItem>
        {
            new SelectListItem {Text = "Pass", Value = "Pass"},
            new SelectListItem {Text = "Credit", Value = "Credit"},
            new SelectListItem {Text = "Merit", Value = "Merit"},
            new SelectListItem {Text = "Distinction", Value = "Distinction"},
            new SelectListItem {Text = "Pass with excellence", Value = "Pass with excellence"},
            new SelectListItem {Text = "No grade awarded", Value = "No grade awarded"}
        };
        public void FromCertificate(Certificate cert)
        {
            BaseFromCertificate(cert);
            SelectedGrade = CertificateData.OverallGrade;
        }

        public Certificate GetCertificateFromViewModel(Certificate certificate, CertificateData data)
        {
            data.OverallGrade = SelectedGrade;
            certificate.CertificateData = JsonConvert.SerializeObject(data);
            return certificate;
        }
    }
}