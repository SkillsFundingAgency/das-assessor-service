using SFA.DAS.AssessorService.ApplyTypes;
using System;
using System.Linq;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class ApplicationResponse 
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid OrganisationId { get; set; }
        public string EndPointAssessorName { get; set; }
        public FinancialGrade financialGrade { get; set; }
        public string ApplicationStatus { get; set; }
        public string ReviewStatus { get; set; }
        public string FinancialReviewStatus { get; set; }
        public ApplyData ApplyData { get; set; }
        public int? StandardCode { get; set; }
        public string CreatedBy { get; set; }
    }
}
