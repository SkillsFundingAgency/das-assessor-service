using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;
using SFA.DAS.AssessorService.AssessmentOrgsImport.exceptions;
using SFA.DAS.AssessorService.AssessmentOrgsImport.models;

namespace SFA.DAS.AssessorService.AssessmentOrgsImport
{
    public class AssessmentOrganisationsReader
    {
        private const string LookupsWorkSheetName = "Lookups";
        private const string OrganisationsWorkSheetName = "Register - Organisations";
        private const int LookupsColumnDeliveryArea = 1;
        private const int LookupsColumnOrganisationType = 2;

        public List<DeliveryArea> HarvestDeliveryAreas(ExcelPackage package)
        {
            var deliveryAreas = new List<DeliveryArea>();

            var worksheet = GetWorksheet(package, LookupsWorkSheetName);

            for (var i = worksheet.Dimension.Start.Row + 1; i <= worksheet.Dimension.End.Row; i++)
            {
                var area = worksheet.Cells[i, LookupsColumnDeliveryArea].Value;
                if (area is null) break;
                if (area.ToString().ToLower() != "all")
                {
                    
                    deliveryAreas.Add(new DeliveryArea { Id = Guid.NewGuid(), Area = area.ToString()});
                }
            }

            if (deliveryAreas.Count == 0)
            {
                throw new NoDataPresentException("Worksheet [{LookupsWorkSheetName}]  contains no delivery areas");  
            }

            return deliveryAreas;
        }

       


        public List<TypeOfOrganisation> HarvestOrganisationTypes(ExcelPackage package)
        {
            var organisationTypes = new List<TypeOfOrganisation>();

            var worksheet = GetWorksheet(package, LookupsWorkSheetName);

            for (var i = worksheet.Dimension.Start.Row + 1; i <= worksheet.Dimension.End.Row; i++)
            {
                var organisationType = worksheet.Cells[i, LookupsColumnOrganisationType].Value;
                if (organisationType is null) break;
                organisationTypes.Add(new TypeOfOrganisation { Id = Guid.NewGuid(), OrganisationType = organisationType.ToString() });
            }

            if (organisationTypes.Count == 0)
            {
                throw new NoDataPresentException("Worksheet [{LookupsWorkSheetName}]  contains no organisation types");
            }

            return organisationTypes;
        }

        public List<EpaOrganisation> HarvestEpaOrganisations(ExcelPackage package, List<TypeOfOrganisation> organisationTypes)
        {
            var organisations = new List<EpaOrganisation>();
            var worksheet = GetWorksheet(package, OrganisationsWorkSheetName);
            for (var i = worksheet.Dimension.Start.Row + 1; i <= worksheet.Dimension.End.Row; i++)
            {

                var epaOrganisationIdentifier = worksheet.Cells[i, 1].Value != null ? worksheet.Cells[i, 1].Value.ToString().Trim() : string.Empty;
                if (epaOrganisationIdentifier == string.Empty)
                        {
                        break;
                        }
                var epaOrganisationName = worksheet.Cells[i, 2].Value != null ? worksheet.Cells[i, 2].Value.ToString() : string.Empty;
                var epaOrganisationType = worksheet.Cells[i, 3].Value != null ? worksheet.Cells[i, 3].Value.ToString() : string.Empty; 
                var epaOrganisationTypeDetails = organisationTypes.First(x => string.Equals(x.OrganisationType,
                        epaOrganisationType?.ToString(), StringComparison.CurrentCultureIgnoreCase));
                if (epaOrganisationType == string.Empty || epaOrganisationTypeDetails.OrganisationType == string.Empty)
                    {
                        if (epaOrganisationType == string.Empty)
                        {
                        throw new NoMatchingOrganisationTypeException(
                            $"There is no stated organisation type for row {i} in worksheet [{OrganisationsWorkSheetName}]");
                    }
                        throw new NoMatchingOrganisationTypeException(
                            $"There is no matching organisation type for [{epaOrganisationType}]");
                    }

                var organisationTypeId = epaOrganisationTypeDetails.Id;
                var websiteLink = worksheet.Cells[i, 4].Value != null ? worksheet.Cells[i, 4].Value.ToString() : string.Empty; 
                var contactAddress1 = worksheet.Cells[i, 5].Value != null ? worksheet.Cells[i, 5].Value.ToString() : string.Empty;
                var contactAddress2 = worksheet.Cells[i, 6].Value != null ? worksheet.Cells[i, 6].Value.ToString() : string.Empty; 
                var contactAddress3 = worksheet.Cells[i, 7].Value != null ? worksheet.Cells[i, 7].Value.ToString() : string.Empty; 
                var contactAddress4 = worksheet.Cells[i, 8].Value != null ? worksheet.Cells[i, 8].Value.ToString() : string.Empty; 
                var contactPostcode = worksheet.Cells[i, 9].Value != null ? worksheet.Cells[i, 9].Value.ToString() : string.Empty; 
                var postcode = ProcessPostcodeForExcessSpaces(contactPostcode);

                var ukprnRead = worksheet.Cells[i, 10].Value != null ? worksheet.Cells[i, 10].Value.ToString() : string.Empty; 
                int.TryParse(ukprnRead, out int ukprnAsInt);
                int? ukprn = null;
                if (ukprnAsInt > 0)
                    ukprn = ukprnAsInt;
                    
                var legalName = worksheet.Cells[i, 11].Value != null ? worksheet.Cells[i, 11].Value.ToString() : string.Empty; 

                organisations.Add(
                        new EpaOrganisation
                        {
                            Id = Guid.NewGuid(),
                            EpaOrganisationIdentifier = epaOrganisationIdentifier,
                            EpaOrganisationName = epaOrganisationName,
                            OrganisationTypeId = organisationTypeId,
                            WebsiteLink = websiteLink,
                            ContactAddress1 = contactAddress1,
                            ContactAddress2 = contactAddress2,
                            ContactAddress3 = contactAddress3,
                            ContactAddress4 = contactAddress4,
                            ContactPostcode = postcode,
                            Ukprn = ukprn,
                            LegalName = legalName
                        });
            }

            return organisations;
        }

        private string ProcessPostcodeForExcessSpaces(object postcodeIn)
        {
            var postcode = postcodeIn?.ToString().Trim();

            if (postcode==null)
                return null;

            if (postcode.Length <= 8)
                return postcode;

            while (postcode.Length > 8)
            {
                var lastSpace = postcode.LastIndexOf(" ", StringComparison.Ordinal);

                if (lastSpace == -1)
                {
                    postcode = postcode.Substring(0, 8);
                    break;
                }

                postcode =
                    postcode.Substring(0, lastSpace > -1 ? lastSpace : postcode.Count()) +
                    postcode.Substring(lastSpace + 1);
            }

            return postcode;
        }


        private static ExcelWorksheet GetWorksheet(ExcelPackage package, string worksheetname)
        {
            var worksheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == worksheetname);

            if (worksheet == null) throw new WorksheetNotAvailableException($"Worksheet [{worksheetname}] not found");
            return worksheet;
        }
    }
}