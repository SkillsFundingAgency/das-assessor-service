using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using System;
using System.Linq;

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

        public string StandardDescriptionWithVersion => (_applicationResponse.ApplyData.Apply.Versions != null && _applicationResponse.ApplyData.Apply.Versions.Any()) ?
            $"{StandardDescription}, Version {_applicationResponse.ApplyData.Apply.Versions.First()}" : StandardDescription;

        public Guid ApplicationId => _applicationResponse.Id;

    }
}
