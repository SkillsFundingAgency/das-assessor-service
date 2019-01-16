using System;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Apply.Financial
{
    public class NewFinancialApplicationViewModel
    {
        public string ApplyingOrganisationName { get; set; }
        public string Status { get; set; }
        public Guid ApplicationId { get; set; }
    }
}