
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateVersionNotApprovedViewModel
    {
        public string AttemptedVersion { get; set; }
        public bool BackToCheckPage { get; set; }
    }
}
