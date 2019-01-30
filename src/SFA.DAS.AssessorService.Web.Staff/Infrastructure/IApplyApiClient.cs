using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure
{
    public interface IApplyApiClient
    {
        Task<List<dynamic>> NewApplications(int sequenceId);
        Task ImportWorkflow(IFormFile file);
        Task<List<dynamic>> GetNewFinancialApplications();
        Task<HttpResponseMessage> Download(Guid applicationId, Guid userId, int sequenceId, int sectionId, string pageId, string questionId, string filename);
        Task<FileInfoResponse> FileInfo(Guid applicationId, Guid userId, int sequenceId, int sectionId, string pageId, string questionId, string filename);
    }
    
    public class FileInfoResponse
    {
        public string Filename { get; set; }
        public string ContentType { get; set; }
    }
}