using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IQnaApiClient
    {
        Task<StartApplicationResponse> StartApplications(StartApplicationRequest startAppRequest);
        Task<ApplicationData> GetApplicationData(Guid applicationId);
        Task<Sequence> GetApplicationActiveSequence(Guid applicationId);
        Task<List<Section>> GetSections(Guid applicationId, Guid sequenceId);
        Task<Section> GetSection(Guid applicationId, Guid sectionId);
        Task<Page> GetPage(Guid applicationId, Guid sectionId, string pageId);
    }
}
