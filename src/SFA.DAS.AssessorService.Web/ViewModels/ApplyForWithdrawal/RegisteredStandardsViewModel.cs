using System;

namespace SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal
{
    public class RegisteredStandardsViewModel
    {
        public string StandardName { get; set; }
        public int Level { get; set; }
        public string ReferenceNumber { get; set; }
        public Guid? ApplicationId { get; set; }
        public int NumVersions { get; set; }
    }
}
