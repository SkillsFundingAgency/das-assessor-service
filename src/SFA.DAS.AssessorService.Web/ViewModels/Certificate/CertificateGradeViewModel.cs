using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateGradeViewModel : CertificateBaseViewModel
    {
        public CertificateGradeViewModel(){}

        public CertificateGradeViewModel(Domain.Entities.Certificate cert) : base(cert)
        {
            SelectedGrade = CertificateData.OverallGrade;
        }

        public void SetUpGrades()
        {
            Grades = new List<SelectListItem>
            {
                new SelectListItem {Text = "Pass", Value = "Pass"},
                new SelectListItem {Text = "Credit", Value = "Credit"},
                new SelectListItem {Text = "Merit", Value = "Merit"},
                new SelectListItem {Text = "Distinction", Value = "Distinction"},
                new SelectListItem {Text = "Pass with excellence", Value = "Pass with excellence"},
                new SelectListItem {Text = "No grade awarded", Value = "No grade awarded"}
            };
        }

        public string SelectedGrade { get; set; }
        public List<SelectListItem> Grades { get; set; }
    }
}