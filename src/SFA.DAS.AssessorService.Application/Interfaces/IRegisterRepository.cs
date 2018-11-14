using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IRegisterRepository
    {
        Task<string> CreateEpaOrganisation(EpaOrganisation organisation);
        Task<string> UpdateEpaOrganisation(EpaOrganisation organisation);
        Task<string> CreateEpaOrganisationStandard(EpaOrganisationStandard organisationStandard, List<int> deliveryAreas);

        Task<string> UpdateEpaOrganisationStandard(EpaOrganisationStandard organisationStandard, List<int> deliveryAreas, string actionChoice);
        Task<string> CreateEpaOrganisationContact(EpaContact contact);
        Task<string> UpdateEpaOrganisationContact(EpaContact contact);
    }
}
