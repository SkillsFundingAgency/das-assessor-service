using System;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateDateViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public DateTime Date { get; set; }
        public string Day { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public DateTime StartDate { get; set; }
        public bool WarningShown { get; set; }

        public void FromCertificate(Domain.Entities.Certificate cert)
        {
            BaseFromCertificate(cert);
            Day = CertificateData.AchievementDate?.Day.ToString();
            Month = CertificateData.AchievementDate?.Month.ToString();
            Year = CertificateData.AchievementDate?.Year.ToString();
            StartDate = CertificateData.LearningStartDate;
        }

        public Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData data)
        {
            data.AchievementDate = new DateTime(int.Parse(Year), int.Parse(Month), int.Parse(Day));

            certificate.CertificateData = JsonConvert.SerializeObject(data);

            return certificate;
        }
    }
}