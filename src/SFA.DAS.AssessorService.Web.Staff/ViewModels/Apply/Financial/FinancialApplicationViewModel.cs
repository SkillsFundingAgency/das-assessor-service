using System;
using SFA.DAS.AssessorService.ApplyTypes;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Apply.Financial
{
    public class FinancialApplicationViewModel
    {
        public FinancialApplicationViewModel()
        {
            Grade = new FinancialApplicationGrade();
        }
        
        public ApplicationSection Section { get; set; }
        public FinancialApplicationGrade Grade { get; set; }
        public Guid ApplicationId { get; set; }
        public Organisation Organisation { get; set; }
    }
}