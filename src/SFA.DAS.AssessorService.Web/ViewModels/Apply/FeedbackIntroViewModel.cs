using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using System;

namespace SFA.DAS.AssessorService.Web.ViewModels.Apply
{
    public class FeedbackIntroViewModel
    {
        private readonly ApplicationResponse _applicationResponse;
        
        public FeedbackIntroViewModel(ApplicationResponse applicationResponse)
        {
            _applicationResponse = applicationResponse;
        }

        public bool IsWithdrawalApplication => _applicationResponse.IsWithdrawalApplication;

        public bool IsStandardWithdrawalApplication => _applicationResponse.IsStandardWithdrawalApplication;

        public bool IsOrganisationWithdrawalApplication => _applicationResponse.IsOrganisationWithdrawalApplication;

        public string StandardDescription => _applicationResponse.ApplyData.Apply.StandardWithReference;

        public Guid ApplicationId => _applicationResponse.Id;
        
    }
}
