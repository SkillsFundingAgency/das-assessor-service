using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
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
        public void FromCertificate(Domain.Entities.Certificate cert)
        {
            BaseFromCertificate(cert);
            SelectedGrade = CertificateData.OverallGrade;
        }

        public CertificateData GetCertificateDataFromViewModel(CertificateData data)
        {
            data.OverallGrade = SelectedGrade;
            return data;
        }
    }
}