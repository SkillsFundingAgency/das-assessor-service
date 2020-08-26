using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.DTOs;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IEMailTemplateQueryRepository
    {
        Task<EmailTemplateSummary> GetEmailTemplate(string templateName);
    }
}