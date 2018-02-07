using System.Threading.Tasks;
using SFA.DAS.AssessorService.Web.ViewModels;

namespace SFA.DAS.AssessorService.Web.Services
{
    public interface IOrganisationService
    {
        Task<Organisation> GetOrganisation(string token);
    }
}