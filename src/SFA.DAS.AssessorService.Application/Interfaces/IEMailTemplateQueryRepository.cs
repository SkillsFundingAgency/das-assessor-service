using SFA.DAS.AssessorService.Domain.DTOs;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IEMailTemplateQueryRepository
    {
        Task<EmailTemplateSummary> GetEmailTemplate(string templateName);
    }
}