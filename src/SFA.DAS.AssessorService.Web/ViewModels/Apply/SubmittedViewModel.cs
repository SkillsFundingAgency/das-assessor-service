using SFA.DAS.AssessorService.Api.Types.Models.Apply;

namespace SFA.DAS.AssessorService.Web.ViewModels.Apply
{
    public class SubmittedViewModel
    {
        private readonly ApplicationResponse _applicationResponse;

        public SubmittedViewModel(ApplicationResponse applicationResponse)
        {
            _applicationResponse = applicationResponse;
        }

        public bool IsWithdrawalApplication => _applicationResponse.IsWithdrawalApplication;

        public bool IsStandardWithdrawalApplication => _applicationResponse.IsStandardWithdrawalApplication;

        public bool IsOrganisationWithdrawalApplication => _applicationResponse.IsOrganisationWithdrawalApplication;

        public string ReferenceNumber { get; set; }
        public string FeedbackUrl { get; set; }
        public string StandardName { get; set; }
    }
}
