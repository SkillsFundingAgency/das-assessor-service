using System.Collections.Generic;
using SFA.DAS.AssessorService.Domain.Entities.AssessmentOrganisations;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IAssessmentOrgsRepository
    {
        string TearDownData();
        void WriteDeliveryAreas(List<DeliveryArea> deliveryAreas);
        void WriteOrganisationTypes(List<TypeOfOrganisation> organisationTypes);

        void WriteOrganisations(List<EpaOrganisation> organisations);
        List<EpaOrganisationStandard> WriteEpaOrganisationStandards(List<EpaOrganisationStandard> orgStandards);
        void WriteStandardDeliveryAreas(List<EpaOrganisationStandardDeliveryArea> organisationStandardDeliveryAreas, List<EpaOrganisationStandard> organisationStandards);
        void WriteOrganisationContacts(List<OrganisationContact> contacts);
    }
}
