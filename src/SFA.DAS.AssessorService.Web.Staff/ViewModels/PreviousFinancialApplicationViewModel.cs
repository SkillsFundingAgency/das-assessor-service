using System;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels
{
    public class PreviousFinancialApplicationViewModel
    {
        public Guid ApplicationId { get; set; }
        public string ApplyingOrganisationName { get; set; }
        public string LinkText { get; set; }
        public string Grade { get; set; }
        public string GradedBy { get; set; }
        public DateTime GradedDate { get; set; }
    }
}