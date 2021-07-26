using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.ViewModels.Apply
{
    public class SubmittedViewModel
    {
        private readonly ApplicationResponse _applicationResponse;

        public SubmittedViewModel(ApplicationResponse applicationResponse)
        {
            _applicationResponse = applicationResponse;
        }

        public bool IsInitialApplication => _applicationResponse.IsInitialApplication;
        public bool IsStandardApplication => _applicationResponse.IsStandardApplication;
        public bool IsWithdrawalApplication => _applicationResponse.IsWithdrawalApplication;

        public bool IsStandardWithdrawalApplication => _applicationResponse.IsStandardWithdrawalApplication;

        public bool IsOrganisationWithdrawalApplication => _applicationResponse.IsOrganisationWithdrawalApplication;

        public string ReferenceNumber { get; set; }
        public string FeedbackUrl { get; set; }
        public string StandardName { get; set; }
        public List<string> Versions { get; set; }
    }
}
