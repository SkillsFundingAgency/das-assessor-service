using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.ApplyTypes;

namespace SFA.DAS.AssessorService.Web.ViewModels.Apply
{
    public class SubmittedViewModel
    {
        public string ReferenceNumber { get; set; }
        public string FeedbackUrl { get; set; }
        public string StandardName { get; set; }
        public ApplicationSubmissionType ApplicationSubmissionType { get; set; } = ApplicationSubmissionType.General;
    }

    public enum ApplicationSubmissionType
    {
        General,
        WithdrawalFromStandard,
        WithdrawalFromRegister
    }

}
