﻿using System;
using System.Collections.Generic;
using System.Text;
using OfficeOpenXml;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities.AssessmentOrganisations;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IAssessmentOrgsSpreadsheetReader
    {
        List<DeliveryArea> HarvestDeliveryAreas(ExcelPackage package);
        List<TypeOfOrganisation> HarvestOrganisationTypes(ExcelPackage package);
        List<EpaOrganisation> HarvestEpaOrganisations(ExcelPackage package, List<TypeOfOrganisation> organisationTypes);
        List<ApprenticeshipStandard> HarvestStandards(ExcelPackage package);
        List<EpaOrganisationStandard> HarvestEpaOrganisationStandards(ExcelPackage package,
            List<EpaOrganisation> epaOrganisations, List<ApprenticeshipStandard> standards);

        List<EpaOrganisationStandardDeliveryArea> HarvestStandardDeliveryAreas(ExcelPackage package,
            List<EpaOrganisation> epaOrganisations, List<ApprenticeshipStandard> standards, List<DeliveryArea> deliveryAreas);

        List<OrganisationContact> GatherOrganisationContacts(List<EpaOrganisation> organisations, List<EpaOrganisationStandard> organisationStandards);
    }
}
