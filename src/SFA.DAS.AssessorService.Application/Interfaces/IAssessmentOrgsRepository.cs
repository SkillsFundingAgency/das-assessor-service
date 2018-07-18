using System.Collections.Generic;
using System.Text;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IAssessmentOrgsRepository
    {
        void TearDownData();
        List<DeliveryArea> WriteDeliveryAreas(List<DeliveryArea> deliveryAreas);
        List<TypeOfOrganisation> WriteOrganisationTypes(List<TypeOfOrganisation> organisationTypes);
        List<EpaOrganisation> WriteOrganisations(List<EpaOrganisation> organisations);

        void WriteEpaOrganisationStandards(List<EpaOrganisationStandard> orgStandards);

        void WriteStandardDeliveryAreas(List<EpaOrganisationStandardDeliveryArea> organisationStandardDeliveryAreas);

        void WriteOrganisationContacts(List<OrganisationContact> contacts);
    }
}
