using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IEMailTemplateQueryRepository
    {
        Task<EMailTemplate> GetEMailTemplate(string templateName);
    }
}