using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IEmailApiClient
    {
        Task<EMailTemplate> GetEmailTemplate(string templateName);
        Task SendEmailWithTemplate(SendEmailRequest sendEmailRequest);
    }
}
