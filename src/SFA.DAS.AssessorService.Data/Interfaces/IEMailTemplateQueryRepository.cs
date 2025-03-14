using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.DTOs;

namespace SFA.DAS.AssessorService.Data.Interfaces
{
    public interface IEMailTemplateQueryRepository
    {
        Task<EmailTemplateSummary> GetEmailTemplate(string templateName);
    }
}