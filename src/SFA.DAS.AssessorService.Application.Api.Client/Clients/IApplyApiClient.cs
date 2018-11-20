using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IApplyApiClient
    {
        Task ImportWorkflow(IFormFile file);
    }
}