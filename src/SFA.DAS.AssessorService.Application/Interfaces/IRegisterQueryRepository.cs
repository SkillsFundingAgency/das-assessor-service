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
        Task<IEnumerable<AparSummary>> GetAssessmentOrganisations();
        Task<IEnumerable<EpaOrganisation>> GetAssessmentOrganisationsByStandardId(int standardId);
        
        Task<IEnumerable<AssessmentOrganisationContact>> GetAssessmentOrganisationContacts(string organisationId);
        Task<AssessmentOrganisationContact> GetAssessmentOrganisationContact(Guid contactId);
        Task<AssessmentOrganisationContact> GetPrimaryOrFirstContact(string organisationId);
        Task<IEnumerable<AparSummary>> GetAssessmentOrganisationsByUkprn(string ukprn);
        Task<IEnumerable<AparSummary>> GetAssessmentOrganisationsByOrganisationId(string organisationId);

        Task<AparSummary> GetAssessmentOrganisationByContactEmail(string email);
        Task<IEnumerable<AparSummary>> GetAssessmentOrganisationsByNameOrCharityNumberOrCompanyNumber(string organisationName);
        Task<IEnumerable<OrganisationStandardSummary>> GetAllOrganisationStandardByOrganisationId(string organisationId);
        Task<OrganisationStandard> GetOrganisationStandardFromOrganisationStandardId(int organisationStandardId);

        Task<IEnumerable<AparSummaryItem>> GetAparSummaryByUkprn(int ukprn);
        Task<IEnumerable<AparSummaryItem>> GetAparSummary();

        Task<IEnumerable<AppliedStandardVersion>> GetAppliedStandardVersionsForEPAO(string organisationId, string standardReference);

        Task<EpaContact> GetContactByContactId(Guid contactId);
        Task<EpaContact> GetContactByEmail(string email);
        Task<EpaContact> GetContactBySignInId(string signinId);
        Task<IEnumerable<OrganisationStandardDeliveryArea>> GetDeliveryAreasByOrganisationStandardId(int organisationStandardId);

        Task<string> GetEpaOrgIdByEndPointAssessmentName(string name);

        Task<int?> AparSummaryUpdate();
        Task<DateTime> GetAparSummaryLastUpdated();
    }
}
