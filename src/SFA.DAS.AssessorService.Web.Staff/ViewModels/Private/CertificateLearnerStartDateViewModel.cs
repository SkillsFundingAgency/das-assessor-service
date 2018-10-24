using System;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private
{
    public class CertificateLearnerStartDateViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public DateTime Date { get; set; }
        public string Day { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public DateTime? StartDate { get; set; }
        public string WarningShown { get; set; }

        public void FromCertificate(Domain.Entities.Certificate cert)
        {
            BaseFromCertificate(cert);
            Day = CertificateData.LearningStartDate?.Day.ToString();
            Month = CertificateData.LearningStartDate?.Month.ToString();
            Year = CertificateData.LearningStartDate?.Year.ToString();
            StartDate = CertificateData.LearningStartDate;
            WarningShown = "false";
        }

        public Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData data)
        {
            data.LearningStartDate = new DateTime(int.Parse(Year), int.Parse(Month), int.Parse(Day));

            certificate.CertificateData = JsonConvert.SerializeObject(data);

            return certificate;
        }
    }
}