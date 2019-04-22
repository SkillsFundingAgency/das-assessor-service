using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SFA.DAS.AssessorService.Web.Staff.Services;
using SFA.DAS.AssessorService.ApplyTypes;

namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure
{
    public interface IApplyApiClient
    {
        Task ImportWorkflow(IFormFile file);
        Task<HttpResponseMessage> Download(Guid applicationId, Guid userId, int sequenceId, int sectionId, string pageId, string questionId, string filename);
        Task<FileInfoResponse> FileInfo(Guid applicationId, Guid userId, int sequenceId, int sectionId, string pageId, string questionId, string filename);
        Task<GetAnswersResponse> GetAnswer(Guid applicationId, string questionTag);
        Task<ApplyTypes.Application> GetApplication(Guid applicationId);
        Task<ApplicationSequence> GetActiveSequence(Guid applicationId);
        Task<ApplicationSequence> GetSequence(Guid applicationId, int sequenceId);
        Task<ApplicationSection> GetSection(Guid applicationId, int sequenceId, int sectionId);
        Task<Organisation> GetOrganisationForApplication(Guid applicationId);
        Task<Contact> GetContact(Guid contactId);
        Task UpdateRoEpaoApprovedFlag(Guid applicationId, Guid contactId, string endPointAssessorOrganisationId,
            bool roEpaoApprovedFlag);
    }
    
    public class FileInfoResponse
    {
        public string Filename { get; set; }
        public string ContentType { get; set; }
    }
}