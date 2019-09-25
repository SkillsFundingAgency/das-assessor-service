using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IApplicationApiClient
    {
        Task<List<ApplicationResponse>> GetApplications(Guid userId, bool createdBy);
        Task<ApplicationResponse> GetApplication(Guid id);
        Task<Guid> CreateApplication(CreateApplicationRequest createApplicationRequest);
        Task<bool> Submit(SubmitApplicationRequest submitApplicationRequest);

        //Task DeleteAnswer(Guid applicationId, int sequenceId, int sectionId, string pageId, Guid answerId, Guid userId);
        Task<bool> UpdateInitialStandardData(Guid Id, int standardCode,string referenceNumber, string standardName);

        Task<List<StandardCollation>> GetStandards();
        Task<List<Option>> GetQuestionDataFedOptions();
        //Task DeleteFile(Guid applicationId, Guid userId, int sequenceId, int sectionId, string pageId, string questionId);

    }
}
