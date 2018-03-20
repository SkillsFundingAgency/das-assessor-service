using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    public class CertificateGradeViewModel : CertificateBaseViewModel
    {
        public CertificateGradeViewModel(){}

        public CertificateGradeViewModel(Certificate cert) : base(cert)
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

    public class CertificateBaseViewModel
    {
        protected CertificateData CertificateData;

        public CertificateBaseViewModel() { }

        public CertificateBaseViewModel(Certificate cert)
        {
            CertificateData = JsonConvert.DeserializeObject<CertificateData>(cert.CertificateData);
            Id = cert.Id;
            GivenNames = CertificateData.LearnerGivenNames;
            FamilyName = CertificateData.LearnerFamilyName;
            Standard = CertificateData.StandardName;
        }
        public Guid Id { get; set; }
        public string FamilyName { get; set; }
        public string GivenNames { get; set; }
        public string Standard { get; set; }
    }
}