using System;
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
        Task<string> UpdateEpaOrganisationStandardAndOrganisationStandardVersions(EpaOrganisationStandard organisationStandard, List<int> deliveryAreas);
        Task<string> CreateEpaOrganisationContact(EpaContact contact);
        Task<string> UpdateEpaOrganisationContact(EpaContact contact, string actionChoice);
        Task<string> AssociateOrganisationWithContact(Guid id, EpaOrganisation org, string status, string actionChoice);
        Task<string> AssociateAllPrivilegesWithContact(EpaContact contact);
        Task UpdateEpaOrganisationPrimaryContact(Guid contactId, string contactUsername);
        Task<string> AssociateDefaultPrivilegesWithContact(EpaContact contact);
    }
}
