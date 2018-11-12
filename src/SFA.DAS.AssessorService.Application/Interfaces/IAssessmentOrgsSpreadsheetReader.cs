using System.Collections.Generic;
using System.Threading.Tasks;
using OfficeOpenXml;
using SFA.DAS.AssessorService.Domain.Entities.AssessmentOrganisations;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IAssessmentOrgsSpreadsheetReader
    {
        List<DeliveryArea> HarvestDeliveryAreas();
        List<TypeOfOrganisation> HarvestOrganisationTypes();
        List<EpaOrganisation> HarvestEpaOrganisations(ExcelPackage package, List<TypeOfOrganisation> organisationTypes);
        List<ApprenticeshipStandard> HarvestStandards(ExcelPackage package);
        List<EpaOrganisationStandard> HarvestEpaOrganisationStandards(ExcelPackage package,
            List<EpaOrganisation> epaOrganisations, List<ApprenticeshipStandard> standards);
        List<EpaOrganisationStandardDeliveryArea> HarvestStandardDeliveryAreas(ExcelPackage package,
            List<EpaOrganisation> epaOrganisations, List<ApprenticeshipStandard> standards, List<DeliveryArea> deliveryAreas);
        List<OrganisationContact> HarvestOrganisationContacts(List<EpaOrganisation> organisations, 
                                                             List<EpaOrganisationStandard> organisationStandards);

        void MapDeliveryAreasCommentsIntoOrganisationStandards(List<EpaOrganisationStandardDeliveryArea> osDeliveryAreas, List<EpaOrganisationStandard> organisationStandards);
        void MapPrimaryContacts(List<EpaOrganisation> organisations, List<OrganisationContact> contacts);
    }
}
