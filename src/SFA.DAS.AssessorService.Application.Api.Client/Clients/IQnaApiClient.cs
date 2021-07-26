using Microsoft.AspNetCore.Http;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IQnaApiClient
    {
        Task<StartApplicationResponse> StartApplications(StartApplicationRequest startAppRequest);
        Task<ApplicationData> GetApplicationData(Guid applicationId);
        Task<Dictionary<string, object>> GetApplicationDataDictionary(Guid applicationId);
        Task<ApplicationData> UpdateApplicationData(Guid applicationId, ApplicationData applicationData);
        Task<Sequence> GetApplicationActiveSequence(Guid applicationId);
        Task<List<Sequence>> GetAllApplicationSequences(Guid applicationId);
        Task<Sequence> GetSequence(Guid applicationId, Guid sequenceId);
        Task<Sequence> GetSequenceBySequenceNo(Guid applicationId, int sequenceNo);
        Task<List<Section>> GetSections(Guid applicationId, Guid sequenceId);
        Task<Section> GetSection(Guid applicationId, Guid sectionId);
        Task<Section> GetSectionBySectionNo(Guid applicationId, int sequenceNo, int sectionNo);
        Task<Page> GetPage(Guid applicationId, Guid sectionId, string pageId);
        Task<Page> GetPageBySectionNo(Guid applicationId, int sequenceNo, int sectionNo, string pageId);
        Task<SetPageAnswersResponse> SetPageAnswers(Guid applicationId, Guid sectionId, string pageId, List<Answer> answer);
        Task<ResetPageAnswersResponse> ResetSectionAnswers(Guid applicationId, int sequenceId, int sectionId);
        Task<AddPageAnswerResponse> AddAnswersToMultipleAnswerPage(Guid applicationId, Guid sectionId, string pageId, List<Answer> answer);
        Task<SetPageAnswersResponse> Upload(Guid applicationId, Guid sectionId, string pageId, IFormFileCollection files);
        Task<HttpResponseMessage> DownloadFile(Guid applicationId, Guid sectionId, string pageId, string questionId, string fileName);
        Task DeleteFile(Guid applicationId, Guid sectionId, string pageId, string questionId, string fileName);
        Task<Page> RemovePageAnswer(Guid applicationId, Guid sectionId, string pageId, Guid answerId);
        Task<bool> AllFeedbackCompleted(Guid applicationId, Guid sequenceId);
    }
}
