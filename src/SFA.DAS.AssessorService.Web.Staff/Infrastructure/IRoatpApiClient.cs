
namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure
{
    using SFA.DAS.AssessorService.Api.Types.Models.Roatp;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    public interface IRoatpApiClient
    {
       Task<IEnumerable<IDictionary<string, object>>> GetCompleteRegister();
       Task<IEnumerable<IDictionary<string, object>>> GetAuditHistory();
       Task<IEnumerable<ProviderType>> GetProviderTypes();
       Task<IEnumerable<OrganisationType>> GetOrganisationTypes(int providerTypeId);
       Task CreateOrganisation(CreateOrganisationRequest organisationRequest);
    }
}
