using System;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels
{
    public class NewFinancialApplicationViewModel
    {
        public string ApplyingOrganisationName { get; set; }
        public string Status { get; set; }
        public string LinkText { get; set; }
        public Guid ApplicationId { get; set; }
    }
}