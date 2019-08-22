using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.QnA.Api.Types.Page;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IApplicationApiClient
    {
        Task<List<ApplicationResponse>> GetApplications(Guid userId, bool createdBy);
        Task<ApplicationResponse> GetApplication(Guid applicationId);
        Task<Guid> CreateApplication(CreateApplicationRequest createApplicationRequest);

        //Task<UploadResult> Upload(Guid applicationId, string userId, int sequenceId, int sectionId, string pageId, IFormFileCollection files);

        //Task<HttpResponseMessage> Download(Guid applicationId, Guid userId, int sequenceId, int sectionId, string pageId, string questionId, string filename);
        //Task<FileInfoResponse> FileInfo(Guid applicationId, Guid userId, int sequenceId, int sectionId, string pageId, string questionId, string filename);
        //Task<ApplicationSequence> GetSequence(Guid applicationId, Guid userId);
        //Task<ApplicationSection> GetSection(Guid applicationId, int sequenceId, int sectionId, Guid userId);

        //Task<Domain.Apply.Page> GetPage(Guid applicationId, int sequenceId, int sectionId, string pageId, Guid userId);

        //Task<UpdatePageAnswersResult> UpdatePageAnswers(Guid applicationId, Guid userId, int sequenceId, int sectionId,
        //    string pageId, List<Answer> answers, bool saveNewAnswers);

        //Task<StartApplicationResponse> StartApplication(Guid userId);
        //Task<bool> Submit(Guid applicationId, int sequenceId, Guid userId, string userEmail);
        //Task DeleteAnswer(Guid applicationId, int sequenceId, int sectionId, string pageId, Guid answerId, Guid userId);
        //Task ImportWorkflow(IFormFile file);
        //Task UpdateApplicationData<T>(T applicationData, Guid applicationId);
        //Task<Domain.Entities.Application> GetApplication(Guid applicationId);

        //Task<string> GetApplicationStatus(Guid applicationId, int standardCode);

        //Task<List<StandardCollation>> GetStandards();
        Task<List<Option>> GetQuestionDataFedOptions();
        //Task DeleteFile(Guid applicationId, Guid userId, int sequenceId, int sectionId, string pageId, string questionId);

    }
}
