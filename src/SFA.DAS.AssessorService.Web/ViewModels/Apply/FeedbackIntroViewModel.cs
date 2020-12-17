using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Domain.Consts;
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

        public bool IsWithdrawalApplication => IsStandardWithdrawalApplication || IsOrganisationWithdrawalApplication;
        
        public bool IsStandardWithdrawalApplication => _applicationResponse.ApplicationType == ApplicationTypes.StandardWithdrawal;
        
        public bool IsOrganisationWithdrawalApplication => _applicationResponse.ApplicationType == ApplicationTypes.OrganisationWithdrawal;

        public string StandardName => $"{_applicationResponse.ApplyData.Apply.StandardName} ({_applicationResponse.ApplyData.Apply.StandardReference})";

        public Guid ApplicationId => _applicationResponse.Id;
        
    }
}
