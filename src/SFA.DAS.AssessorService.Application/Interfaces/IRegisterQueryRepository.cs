using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IRegisterQueryRepository
    {
        Task<IEnumerable<OrganisationType>> GetOrganisationTypes();
        Task<IEnumerable<DeliveryArea>> GetDeliveryAreas();
        Task<EpaOrganisation> GetEpaOrganisationById(Guid id);
        Task<EpaOrganisation> GetEpaOrganisationByOrganisationId(string organisationId);


        Task<string> EpaOrganisationIdCurrentMaximum();
        Task<int> EpaContactUsernameHighestCounter();
        Task<IEnumerable<AssessmentOrganisationSummary>> GetAssessmentOrganisations();
        Task<IEnumerable<EpaOrganisation>> GetAssessmentOrganisationsByStandardId(int standardId);
        
        Task<IEnumerable<AssessmentOrganisationContact>> GetAssessmentOrganisationContacts(string organisationId);
        Task<AssessmentOrganisationContact> GetAssessmentOrganisationContact(string contactId);
        Task<AssessmentOrganisationContact> GetPrimaryOrFirstContact(string organisationId);
        Task<IEnumerable<OrganisationStandardSummary>> GetOrganisationStandardByOrganisationId(string organisationId);
        Task<IEnumerable<OrganisationStandardPeriod>> GetOrganisationStandardPeriodsByOrganisationStandard(string organisationId, int standardId);
        Task<IEnumerable<AssessmentOrganisationSummary>> GetAssessmentOrganisationsByUkprn(string ukprn);
        Task<IEnumerable<AssessmentOrganisationSummary>> GetAssessmentOrganisationsByOrganisationId(string organisationId);
        Task<IEnumerable<AssessmentOrganisationSummary>> GetAssessmentOrganisationsbyName(string organisationName);
    }
}
