using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateGradeViewModel : CertificateBaseViewModel
    {
        public string SelectedGrade { get; set; }       

        private List<SelectListItem> grades = new List<SelectListItem>
        {
            new SelectListItem {Text = CertificateGrade.Pass, Value = CertificateGrade.Pass},
            new SelectListItem {Text = CertificateGrade.Credit, Value = CertificateGrade.Credit},
            new SelectListItem {Text = CertificateGrade.Merit, Value = CertificateGrade.Merit},
            new SelectListItem {Text = CertificateGrade.Distinction, Value = CertificateGrade.Distinction},
            new SelectListItem {Text = CertificateGrade.PassWithExcellence, Value = CertificateGrade.PassWithExcellence},
            new SelectListItem {Text = CertificateGrade.Outstanding, Value = CertificateGrade.Outstanding},
            new SelectListItem {Text = CertificateGrade.NoGradeAwarded, Value = CertificateGrade.NoGradeAwarded},
            new SelectListItem {Text = CertificateGrade.Fail, Value = CertificateGrade.Fail}
        };

        public List<SelectListItem> Grades
        {
            get { return grades; }
            set { grades = value; }
        }

        public override void FromCertificate(Domain.Entities.Certificate cert)
        {
            base.FromCertificate(cert);
            SelectedGrade = CertificateData.OverallGrade;

            if(cert?.StandardCode == 201)
            {
                // ON-2352
                int index = Grades.FindIndex(g => g.Value == CertificateGrade.Fail);
                Grades[index].Text = "Inadequate";
            }
        }

        public override Domain.Entities.Certificate GetCertificateFromViewModel(Domain.Entities.Certificate certificate, CertificateData certData)
        {
            certData.OverallGrade = SelectedGrade;
            certificate.CertificateData = JsonConvert.SerializeObject(certData);

            return certificate;
        }
    }
}