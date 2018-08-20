using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IRegisterQueryRepository
    {
        Task<IEnumerable<OrganisationType>> GetOrganisationTypes();
        Task<IEnumerable<DeliveryArea>> GetDeliveryAreas();
        Task<IEnumerable<AssessmentOrganisationSummary>> GetAssessmentOrganisations();
        Task<EpaOrganisation> GetEpaOrganisationByOrganisationId(string organisationId);
        Task<IEnumerable<EpaOrganisation>> GetAssessmentOrganisationsByStandardId(int standardId);
        Task<IEnumerable<AssessmentOrganisationContact>> GetAssessmentOrganisationContacts(string organisationId);
        Task<AssessmentOrganisationContact> GetPrimaryOrFirstContact(string organisationId);
    }
}
