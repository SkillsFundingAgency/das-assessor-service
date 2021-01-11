using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.QnA.Api.Types;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Controllers.Apply
{
    public class ApplicationService : IApplicationService
    {
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IApplicationApiClient _applicationApiClient;
        
        public ApplicationService(IQnaApiClient qnApiClient, IApplicationApiClient applicationApiClient)
        {
            _qnaApiClient = qnApiClient;
            _applicationApiClient = applicationApiClient;
        }

        public async Task<bool> ResetApplicationToStage1(Guid id, Guid userId)
        {
            if(await _applicationApiClient.ResetApplicationToStage1(id, userId))
            {
                var application = await _applicationApiClient.GetApplication(id);
                await _qnaApiClient.ResetSectionAnswers(application.ApplicationId, ApplyConst.STANDARD_SEQUENCE_NO, ApplyConst.STANDARD_DETAILS_SECTION_NO);
                return true;
            }

            return false;
        }
    }
}
