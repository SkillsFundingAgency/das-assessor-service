using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IApplicationApiClient
    {
        Task<List<ApplicationResponse>> GetOrganisationApplications(Guid userId);
        Task<List<ApplicationResponse>> GetStandardApplications(Guid userId);
        Task<List<ApplicationResponse>> GetWithdrawalApplications(Guid userId);
        Task<List<ApplicationResponse>> GetOrganisationWithdrawalApplications(Guid userId);
        Task<List<ApplicationResponse>> GetStandardWithdrawalApplications(Guid userId);
        Task<ApplicationResponse> GetApplication(Guid id);
        Task<ApplicationResponse> GetApplicationForUser(Guid id, Guid userId);
        Task<DateTime?> GetLatestWithdrawalDateForStandard(Guid organisationId, int? standardCode);

        Task<Guid> CreateApplication(CreateApplicationRequest createApplicationRequest);
        Task DeleteApplications(DeleteApplicationsRequest deleteApplicationsRequest);
        Task<bool> SubmitApplicationSequence(SubmitApplicationSequenceRequest submitApplicationRequest);

        Task<bool> UpdateStandardData(Guid Id, int standardCode, string referenceNumber, string standardName, List<string> versions, string standardApplicationType = null);
        Task<bool> ResetApplicationToStage1(Guid applicationId);

        Task<List<DeliveryArea>> GetQuestionDataFedOptions();
    }
}
