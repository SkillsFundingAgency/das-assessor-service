using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.ViewModels.Apply
{
    public class SubmittedViewModel
    {
        public string ReferenceNumber { get; set; }
        public string FeedbackUrl { get; set; }
        public string StandardName { get; set; }
        public SubmissionType SubmissionType { get; set; } = SubmissionType.General;
    }

    public enum SubmissionType
    {
        General,
        WithdrawalFromStandard,
        WithdrawalFromRegister
    }

}
