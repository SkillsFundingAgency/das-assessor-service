using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IRegisterApiClient
    {
        Task<bool> AssociateOrganisationWithEpaContact(AssociateEpaOrganisationWithEpaContactRequest request);
        Task<string> CreateEpaContact(CreateEpaOrganisationContactRequest request);
        Task<string> CreateEpaOrganisation(CreateEpaOrganisationRequest request);
        Task<string> CreateEpaOrganisationStandard(CreateEpaOrganisationStandardRequest request);
        Task<AssessmentOrganisationContact> GetEpaContact(string contactId);
        Task<EpaContact> GetEpaContactByEmail(string email);
        Task<EpaOrganisation> GetEpaOrganisation(string organisationId);
        Task<List<OrganisationStandardSummary>> GetEpaOrganisationStandards(string organisationId);
        Task<OrganisationStandard> GetOrganisationStandard(int organisationStandardId);
        Task<List<AssessmentOrganisationSummary>> SearchOrganisations(string searchString);
        Task<List<StandardVersion>> SearchStandards(string searchString);
        Task<string> UpdateEpaContact(UpdateEpaOrganisationContactRequest request);
        Task<string> UpdateEpaOrganisation(UpdateEpaOrganisationRequest request);
        Task<string> UpdateEpaOrganisationStandard(UpdateEpaOrganisationStandardRequest request);
        Task<List<DeliveryArea>> GetDeliveryAreas();
    }
}