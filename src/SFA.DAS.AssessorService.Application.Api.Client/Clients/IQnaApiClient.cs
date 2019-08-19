using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.QnA.Api.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IQnaApiClient
    {
        Task<StartApplicationResponse> StartApplications(StartApplicationRequest startAppRequest);
        Task<ApplicationData> GetApplicationData(Guid applicationId);
        Task<Sequence> GetApplicationActiveSequence(Guid applicationId);
        Task<List<Section>> GetSections(Guid applicationId, Guid sequenceId);
    }
}
