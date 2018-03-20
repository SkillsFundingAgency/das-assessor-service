using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateDateViewModel : CertificateBaseViewModel
    {
        public CertificateDateViewModel(){}

        public CertificateDateViewModel(Domain.Entities.Certificate cert) : base(cert)
        {
            Day = CertificateData.AchievementDate?.Day.ToString();
            Month = CertificateData.AchievementDate?.Month.ToString();
            Year = CertificateData.AchievementDate?.Year.ToString();
        }

        public DateTime Date { get; set; }

        public string Day { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
    }
}