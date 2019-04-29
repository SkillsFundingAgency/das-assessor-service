﻿using System;
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
        Task<AssessmentOrganisationContact> GetAssessmentOrganisationContact(Guid contactId);
        Task<AssessmentOrganisationContact> GetPrimaryOrFirstContact(string organisationId);
        Task<IEnumerable<AssessmentOrganisationSummary>> GetAssessmentOrganisationsByUkprn(string ukprn);
        Task<IEnumerable<AssessmentOrganisationSummary>> GetAssessmentOrganisationsByOrganisationId(string organisationId);

        Task<AssessmentOrganisationSummary> GetAssessmentOrganisationByContactEmail(string email);
        Task<IEnumerable<AssessmentOrganisationSummary>> GetAssessmentOrganisationsByNameOrCharityNumberOrCompanyNumber(string organisationName);
        Task<IEnumerable<int>> GetDeliveryAreaIdsByOrganisationStandardId(int organisationStandardId);
        Task<IEnumerable<OrganisationStandardSummary>> GetOrganisationStandardByOrganisationId(string organisationId);
        Task<OrganisationStandard> GetOrganisationStandardFromOrganisationStandardId(int organisationStandardId);

        Task<EpaContact> GetContactByContactId(Guid contactId);
        Task<EpaContact> GetContactByEmail(string email);
        Task<EpaContact> GetContactBySignInId(string signinId);
        Task<IEnumerable<OrganisationStandardDeliveryArea>> GetDeliveryAreasByOrganisationStandardId(int organisationStandardId);
    }
}
