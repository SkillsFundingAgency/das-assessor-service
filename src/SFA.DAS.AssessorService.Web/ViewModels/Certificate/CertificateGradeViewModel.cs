using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateGradeViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public string SelectedGrade { get; set; }       

        public List<SelectListItem> Grades = new List<SelectListItem>
        {
            new SelectListItem {Text = CertificateGrade.Pass, Value = CertificateGrade.Pass},
            new SelectListItem {Text = CertificateGrade.Credit, Value = CertificateGrade.Credit},
            new SelectListItem {Text = CertificateGrade.Merit, Value = CertificateGrade.Merit},
            new SelectListItem {Text = CertificateGrade.Distinction, Value = CertificateGrade.Distinction},
            new SelectListItem {Text = CertificateGrade.PassWithExcellence, Value = CertificateGrade.PassWithExcellence},
            new SelectListItem {Text = CertificateGrade.Outstanding, value = CertificateGrade.Outstanding},
            new SelectListItem {Text = CertificateGrade.NoGradeAwarded, Value = CertificateGrade.NoGradeAwarded},
            new SelectListItem {Text = CertificateGrade.Fail, Value = CertificateGrade.Fail}
        };
        public override void FromCertificate(Domain.Entities.Certificate cert)
        {
            base.FromCertificate(cert);
            SelectedGrade = CertificateData.OverallGrade;
        }

        public Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData data)
        {
            data.OverallGrade = SelectedGrade;
            certificate.CertificateData = JsonConvert.SerializeObject(data);

            return certificate;
        }
    }
}