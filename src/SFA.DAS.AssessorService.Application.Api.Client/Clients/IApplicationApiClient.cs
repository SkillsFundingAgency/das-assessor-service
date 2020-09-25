using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IApplicationApiClient
    {
        Task<List<ApplicationResponse>> GetCombinedApplications(Guid userId);
        Task<List<ApplicationResponse>> GetOrganisationApplications(Guid userId);
        Task<List<ApplicationResponse>> GetStandardApplications(Guid userId);
        Task<List<ApplicationResponse>> GetWithdrawalApplications(Guid userId);
        Task<List<ApplicationResponse>> GetOrganisationWithdrawalApplications(Guid userId);
        Task<List<ApplicationResponse>> GetStandardWithdrawalApplications(Guid userId);
        Task<ApplicationResponse> GetApplication(Guid id);

        Task<Guid> CreateApplication(CreateApplicationRequest createApplicationRequest);
        Task<bool> SubmitApplicationSequence(SubmitApplicationSequenceRequest submitApplicationRequest);

        Task<bool> UpdateStandardData(Guid Id, int standardCode,string referenceNumber, string standardName);

        Task<List<StandardCollation>> GetStandards();
        Task<List<DeliveryArea>> GetQuestionDataFedOptions();
    }
}
