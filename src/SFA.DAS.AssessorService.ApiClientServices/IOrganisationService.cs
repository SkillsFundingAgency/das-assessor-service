using System.Threading.Tasks;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels;

namespace SFA.DAS.AssessorService.Web.Services
{
    public interface IOrganisationService
    {
        Task<ApiUriGenerator.Organisation> GetOrganisation(string token, int ukprn);
    }
}