using System;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateDateViewModel : CertificateBaseViewModel
    {
        public string SelectedGrade { get; set; }
        public DateTime Date { get; set; }
        public string Day { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public DateTime? StartDate { get; set; }
        public string WarningShown { get; set; }

        public override void FromCertificate(Domain.Entities.Certificate cert)
        {
            base.FromCertificate(cert);

            Day = CertificateData.AchievementDate?.Day.ToString();
            Month = CertificateData.AchievementDate?.Month.ToString();
            Year = CertificateData.AchievementDate?.Year.ToString();
            
            StartDate = CertificateData.LearningStartDate;
            SelectedGrade = CertificateData.OverallGrade;
            WarningShown = "false";
        }

        public override Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData data)
        {
            data.AchievementDate = new DateTime(int.Parse(Year), int.Parse(Month), int.Parse(Day));

            certificate.CertificateData = JsonConvert.SerializeObject(data);

            return certificate;
        }
    }
}