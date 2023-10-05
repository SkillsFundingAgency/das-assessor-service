using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System.Linq;
using SFA.DAS.AssessorService.ApplyTypes;

namespace SFA.DAS.AssessorService.Web.Helpers
{
    public class ApplicationFinder
    {
        private readonly IApplicationApiClient _applicationApiClient;

        public ApplicationFinder(IApplicationApiClient applicationApiClient)
        {
            _applicationApiClient = applicationApiClient;
        }

        public async Task<ApplicationResponse> GetWithdrawalApplicationInProgressForContact(Guid contactId, string referenceNumber)
        {
            var applications = await GetWithdrawalApplications(contactId);
            return applications.FirstOrDefault(a => !string.IsNullOrWhiteSpace(a.StandardReference) &&
                                               a.StandardReference.Equals(referenceNumber, StringComparison.InvariantCultureIgnoreCase) &&
                                               a.ApplyData.Apply.Versions == null);
        }

        private async Task<List<ApplicationResponse>> GetWithdrawalApplications(Guid contactId)
        {
            return (await _applicationApiClient.GetWithdrawalApplications(contactId))
                    .Where(x => x.IsStandardWithdrawalApplication)
                    .Where(x => x.ApplicationStatus != ApplicationStatus.Declined)
                    .Where(x => x.ApplicationStatus != ApplicationStatus.Approved)
                    .Where(x => x.ApplicationStatus != ApplicationStatus.Deleted)
                    .Where(x => x.ApplicationStatus != ApplicationStatus.New)
                    .ToList();

        }
    }
}
