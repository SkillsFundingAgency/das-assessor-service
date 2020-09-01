using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.DTOs;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IEmailApiClient
    {
        Task<EmailTemplateSummary> GetEmailTemplate(string templateName);
        Task SendEmailWithTemplate(SendEmailRequest sendEmailRequest);
    }
}
