using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure
{
    public interface IApplyApiClient
    {
        Task<List<dynamic>> NewApplications();
        Task ImportWorkflow(IFormFile file);
        Task<List<dynamic>> GetNewFinancialApplications();
    }
}