using SFA.DAS.AssessorService.Domain.DTOs.Staff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch
{
    public class GetFrameworkLearnerResponse
    {
        public Guid Id { get; set; }
        public string FrameworkCertificateNumber { get; set; }
        public string ApprenticeForename { get; set; }
        public string ApprenticeSurname { get; set; }
        public DateTime ApprenticeDoB { get; set; }
        public long? ApprenticeULN { get; set; }
        public string FrameworkName { get; set; }
        public string PathwayName { get; set; }
        public string ApprenticeshipLevelName { get; set; }
        public List<QualificationDetails> QualificationsAndAwardingBodies { get; set; }
        public string ProviderName { get; set; }
        public string EmployerName { get; set; }
        public DateTime? ApprenticeStartdate { get; set; }
        public DateTime? ApprenticeLastdateInLearning { get; set; }
        public DateTime CertificationDate { get; set; }
        public string CertificateReference { get; set; }
        public string CertificateStatus { get; set; }
        public DateTime? CertificatePrintStatusAt { get; set; }
        public string CertificatePrintReasonForChange { get; set;  }
        public DateTime? CertificateLastUpdatedAt { get; set; }
        public List<CertificateLogSummary> CertificateLogs{ get; set; }
    }

    public class QualificationDetails
    {
        public string Name { get; set; }
        public string AwardingBody { get; set; }
    }
}
