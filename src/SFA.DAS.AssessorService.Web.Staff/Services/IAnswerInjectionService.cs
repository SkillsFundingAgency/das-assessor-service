using SFA.DAS.AssessorService.Api.Types.Commands;
using SFA.DAS.AssessorService.Api.Types.Models;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Staff.Services
{
    public interface IAnswerInjectionService
    {
        Task<CreateOrganisationAndContactFromApplyResponse> InjectApplyOrganisationAndContactDetailsIntoRegister(CreateOrganisationContactCommand command);
    }
}
