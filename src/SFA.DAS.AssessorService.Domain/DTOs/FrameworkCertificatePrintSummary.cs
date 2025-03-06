namespace SFA.DAS.AssessorService.Domain.DTOs
{
    public class FrameworkCertificatePrintSummary : CertificatePrintSummaryBase
    {
        public string ApprenticeReference { get; set; }
        public string PathwayName { get; set; }
        public string FrameworkName { get; set; }
        public string FrameworkLevelName { get; set; }
    }
}
