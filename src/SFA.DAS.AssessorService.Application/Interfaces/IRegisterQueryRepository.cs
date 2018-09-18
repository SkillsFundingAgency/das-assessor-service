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
        Task<bool> EpaOrganisationExistsWithOrganisationId(string organisationId);
        Task<bool> EpaOrganisationExistsWithUkprn(long ukprn);
        Task<bool> OrganisationTypeExists(int organisationTypeId);
        Task<bool> EpaOrganisationAlreadyUsingUkprn(long ukprn, string organisationId);
        Task<string> EpaOrganisationIdCurrentMaximum();

        Task<bool> ContactIdIsValid(string contactId);
        Task<IEnumerable<AssessmentOrganisationSummary>> GetAssessmentOrganisations();
        Task<bool> EpaOrganisationStandardExists(string organisationId, int standardCode);
        Task<IEnumerable<EpaOrganisation>> GetAssessmentOrganisationsByStandardId(int standardId);
        Task<IEnumerable<AssessmentOrganisationContact>> GetAssessmentOrganisationContacts(string organisationId);
        Task<AssessmentOrganisationContact> GetPrimaryOrFirstContact(string organisationId);
        Task<IEnumerable<OrganisationStandardSummary>> GetOrganisationStandardByOrganisationId(string organisationId);

        Task<IEnumerable<OrganisationStandardPeriod>> GetOrganisatonStandardPeriodsByOrganisationStandard(string organisationId, int standardId);
    }
}
