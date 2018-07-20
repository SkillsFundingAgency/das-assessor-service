using System.Collections.Generic;
using SFA.DAS.AssessorService.Domain.Entities.AssessmentOrganisations;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IAssessmentOrgsRepository
    {
        string TearDownData();

        void SetBuildAction(bool useStringBuilder);
        void WriteDeliveryAreas(List<DeliveryArea> deliveryAreas);
        void WriteOrganisationTypes(List<TypeOfOrganisation> organisationTypes);

        void WriteOrganisations(List<EpaOrganisation> organisations);
        void WriteEpaOrganisationStandards(List<EpaOrganisationStandard> orgStandards);
        void WriteStandardDeliveryAreas(List<EpaOrganisationStandardDeliveryArea> organisationStandardDeliveryAreas);
        void WriteOrganisationContacts(List<OrganisationContact> contacts);
    }
}
