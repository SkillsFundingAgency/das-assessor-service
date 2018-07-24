using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using OfficeOpenXml;
using SFA.DAS.AssessorService.Data.Exceptions;
using SFA.DAS.AssessorService.Domain.Entities.AssessmentOrganisations;

namespace SFA.DAS.AssessorService.Data
{
    public class AssessmentOrgsSpreadsheetReader: IAssessmentOrgsSpreadsheetReader
    {
        private const string LookupsWorkSheetName = "Lookups";
        private const string OrganisationsWorkSheetName = "Register - Organisations";
        private const string EpaStandardsWorkSheetName = "Register - Standards";
        private const string StandardsWorkSheetName = "Standards Lookup (LARS copy)";

        private const string DeliveryAreasWorkSheetName = "Register - Delivery areas";
        private const int LookupsColumnDeliveryArea = 1;
        private const int LookupsColumnOrganisationType = 2;

        public List<DeliveryArea> HarvestDeliveryAreas(ExcelPackage package)
        {
            var deliveryAreas = new List<DeliveryArea>();

            var worksheet = GetWorksheet(package, LookupsWorkSheetName);

            for (var i = worksheet.Dimension.Start.Row + 2; i <= worksheet.Dimension.End.Row; i++)
            {
                var area = worksheet.Cells[i, LookupsColumnDeliveryArea].Value;
                if (area is null) break;
                if (area.ToString().ToLower() != "all")
                {
                    deliveryAreas.Add(new DeliveryArea { Id = i - 2, Area = area.ToString(), Status = "Live" });
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
                organisationTypes.Add(new TypeOfOrganisation { Id = i - 1, OrganisationType = organisationType.ToString(), Status = "Live" });
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
                var epaOrganisationTypeDetails = organisationTypes.FirstOrDefault(x => string.Equals(x.OrganisationType,
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
                var ukprn = ProcessNullableIntValue(ukprnRead);

                var legalName = worksheet.Cells[i, 11].Value != null ? worksheet.Cells[i, 11].Value.ToString() : string.Empty;

                organisations.Add(
                    new EpaOrganisation
                    {
                        Id = Guid.NewGuid(),
                        EndPointAssessorOrganisationId = epaOrganisationIdentifier,
                        EndPointAssessorName = epaOrganisationName,
                        OrganisationTypeId = organisationTypeId,
                        OrganisationData = new OrganisationData {
                                                                LegalName = legalName,
                                                                WebsiteLink = websiteLink,
                                                                Address1 = contactAddress1,
                                                                Address2 = contactAddress2,
                                                                Address3 = contactAddress3,
                                                                Address4 = contactAddress4,
                                                                Postcode = postcode},
                        EndPointAssessorUkprn = ukprn,                    
                        Status = "New"
                    });
            }

            return organisations;
        }

        public List<ApprenticeshipStandard> HarvestStandards(ExcelPackage package)
        {
            var standards = new List<ApprenticeshipStandard>();
            var worksheet = GetWorksheet(package, StandardsWorkSheetName);

            for (var i = worksheet.Dimension.Start.Row + 4; i <= worksheet.Dimension.End.Row; i++)
            {
                var standardCode = ProcessValueAsInt(worksheet.Cells[i, 1].Value?.ToString(), "StandardCode", StandardsWorkSheetName, i);
                var version = ProcessValueAsInt(worksheet.Cells[i, 2].Value?.ToString(), "Version", StandardsWorkSheetName, i);
                var standardName = worksheet.Cells[i, 3].Value?.ToString();
                var standardSectorCode = ProcessValueAsInt(worksheet.Cells[i, 4].Value?.ToString(), "StandardSectorCode", StandardsWorkSheetName, i);
                var notionalEndLevel = ProcessValueAsInt(worksheet.Cells[i, 5].Value?.ToString(), "NotionalEndLevel", StandardsWorkSheetName, i);
                var effectiveFrom = ProcessValueAsDateTime(worksheet.Cells[i, 6].Value?.ToString(), "EffectiveFrom", StandardsWorkSheetName, i);
                var effectiveTo = ProcessNullableDateValue(worksheet.Cells[i, 7].Value?.ToString());
                var lastDateStarts = ProcessNullableDateValue(worksheet.Cells[i, 8].Value?.ToString());
                var urlLink = worksheet.Cells[i, 9].Value?.ToString();
                var sectorSubjectAreaTier1 = ProcessNullableIntValue(worksheet.Cells[i, 10].Value?.ToString());
                var sectorSubjectAreaTier2 = worksheet.Cells[i, 11].Value?.ToString();
                var integratedDegreeStandard = ProcessYesNoValuesIntoBoolean(worksheet.Cells[i, 12].Value?.ToString());

                var createdOn = ProcessNullableDateValue(worksheet.Cells[i, 13].Value?.ToString());
                var createdBy = worksheet.Cells[i, 14].Value?.ToString();

                var modifiedOn = ProcessNullableDateValue(worksheet.Cells[i, 15].Value?.ToString());
                var modifiedBy = worksheet.Cells[i, 16].Value?.ToString();

                standards.Add(
                    new ApprenticeshipStandard
                    {
                        StandardCode = standardCode,
                        Version = version,
                        StandardName = standardName,
                        StandardSectorCode = standardSectorCode,
                        NotionalEndLevel = notionalEndLevel,
                        EffectiveFrom = effectiveFrom,
                        EffectiveTo = effectiveTo,
                        LastDateStarts = lastDateStarts,
                        UrlLink = urlLink,
                        SectorSubjectAreaTier1 = sectorSubjectAreaTier1,
                        SectorSubjectAreaTier2 = sectorSubjectAreaTier2,
                        IntegratedDegreeStandard = integratedDegreeStandard,
                        CreatedOn = createdOn,
                        CreatedBy = createdBy,
                        ModifiedOn = modifiedOn,
                        ModifiedBy = modifiedBy,
                        Status = "Live"
                    });
            }

            return standards;
        }

        public List<EpaOrganisationStandard> HarvestEpaOrganisationStandards(ExcelPackage package, List<EpaOrganisation> epaOrganisations, List<ApprenticeshipStandard> standards)
        {
            var epaOrganisationStandards = new List<EpaOrganisationStandard>();
            var worksheet = GetWorksheet(package, EpaStandardsWorkSheetName);
            for (var i = worksheet.Dimension.Start.Row + 1; i <= worksheet.Dimension.End.Row; i++)
            {
                var epaOrganisationIdentifier = worksheet.Cells[i, 1].Value?.ToString();
                var status = "Live";
                if (epaOrganisationIdentifier == null || epaOrganisations.All(x => x.EndPointAssessorOrganisationId != epaOrganisationIdentifier))
                    continue;

                var standardCode = ProcessNullableIntValue(worksheet.Cells[i, 3].Value?.ToString());
                 
                if (standardCode == null || standards.All(x => x.StandardCode != standardCode))
                    continue;

                var effectiveFrom = ProcessNullableDateValue(worksheet.Cells[i, 5].Value?.ToString());
                var effectiveTo = ProcessNullableDateValue(worksheet.Cells[i, 6].Value?.ToString());
                var contactName = worksheet.Cells[i, 7].Value?.ToString();
                var contactPhoneNumber = worksheet.Cells[i, 8].Value?.ToString();
                var contactEmail = worksheet.Cells[i, 9].Value?.ToString();
                var dateStandardApprovedOnRegister = ProcessNullableDateValue(worksheet.Cells[i, 10].Value?.ToString());
                var comments = worksheet.Cells[i, 11].Value?.ToString();

                epaOrganisationStandards.Add(
                    new EpaOrganisationStandard
                    {
                        EndPointAssessorOrganisationId = epaOrganisationIdentifier,
                        StandardCode = standardCode.Value,
                        EffectiveFrom = effectiveFrom,
                        EffectiveTo = effectiveTo,
                        ContactName = contactName,
                        ContactPhoneNumber = contactPhoneNumber,
                        ContactEmail = contactEmail,
                        DateStandardApprovedOnRegister = dateStandardApprovedOnRegister,
                        Comments = comments,
                        Status = status
                    }
                );
            }
            return epaOrganisationStandards;
        }

        public List<EpaOrganisationStandardDeliveryArea> HarvestStandardDeliveryAreas(ExcelPackage package, List<EpaOrganisation> epaOrganisations, List<ApprenticeshipStandard> standards, List<DeliveryArea> deliveryAreas)
        {
            var standardDeliveryAreas = new List<EpaOrganisationStandardDeliveryArea>();
            var worksheet = GetWorksheet(package, DeliveryAreasWorkSheetName);
            for (var i = worksheet.Dimension.Start.Row + 1; i <= worksheet.Dimension.End.Row; i++)
            {
                var epaOrganisationIdentifier = worksheet.Cells[i, 1].Value?.ToString();
                if (epaOrganisationIdentifier == null || epaOrganisations.All(x => x.EndPointAssessorOrganisationId != epaOrganisationIdentifier))
                    continue;

                var standardCode = ProcessNullableIntValue(worksheet.Cells[i, 3].Value?.ToString());

                if (standardCode == null)
                {
                    var standardName = worksheet.Cells[i, 4].Value?.ToString();
                    var res = standards.Where(x => x.StandardName == standardName).ToList();

                    if (res.Count() == 1)
                    {
                        standardCode = res?.First()?.StandardCode;
                    }
                }

                if (standardCode == null || standards.All(x => x.StandardCode != standardCode.Value))
                    continue;

                var deliveryArea = worksheet.Cells[i, 5].Value != null
                    ? worksheet.Cells[i, 5].Value.ToString().Trim()
                    : string.Empty;

                var comments = worksheet.Cells[i, 6].Value != null
                    ? worksheet.Cells[i, 6].Value.ToString().Trim()
                    : string.Empty;

                foreach (var delArea in deliveryAreas.Where(x => deliveryArea.Contains(x.Area) || deliveryArea == "All"))
                {
                    var standardDeliveryArea = new EpaOrganisationStandardDeliveryArea
                    {
                        EndPointAssessorOrganisationId = epaOrganisationIdentifier,
                        StandardCode = standardCode.Value,
                        DeliveryAreaId = delArea.Id,
                        Comments = comments,
                        Status = "Live"
                    };

                    if (!standardDeliveryAreas.Any(x => x.EndPointAssessorOrganisationId == standardDeliveryArea.EndPointAssessorOrganisationId
                                                        && x.StandardCode == standardDeliveryArea.StandardCode
                                                        && x.DeliveryAreaId == standardDeliveryArea.DeliveryAreaId))
                                {
                                standardDeliveryAreas.Add(standardDeliveryArea);
                                }
                }
            }
            return standardDeliveryAreas;
        }

        public List<OrganisationContact> HarvestOrganisationContacts(List<EpaOrganisation> organisations, List<EpaOrganisationStandard> organisationStandards)
        {

            var distinctContacts = (from orgStandard in organisationStandards
                    .Where(o => !(string.IsNullOrEmpty(o?.ContactName?.Trim())
                                  && string.IsNullOrEmpty(o?.ContactEmail?.Trim())))
                select new OrganisationContact
                {
                    DisplayName = orgStandard?.ContactName?.Trim() ?? "unknown",
                    Email = orgStandard?.ContactEmail?.Trim(),
                    PhoneNumber = orgStandard?.ContactPhoneNumber,
                    OrganisationId = organisations.First(x => x.EndPointAssessorOrganisationId == orgStandard.EndPointAssessorOrganisationId).Id,
                    EndPointAssessorOrganisationId = orgStandard.EndPointAssessorOrganisationId,
                    Status = "New",
                }).GroupBy(o => new
            {
                o.DisplayName,
                o.Email,
                o.PhoneNumber,
                o.OrganisationId,
                o.EndPointAssessorOrganisationId,
                o.Status
            });

            var contacts = new List<OrganisationContact>(); ;

            var ctr = 0;
            foreach (var cont in distinctContacts.ToList())
            {

                var contact = cont.Key;
                ctr++;
                contacts.Add(
                    new OrganisationContact
                    {
                        DisplayName = contact.DisplayName,
                        Email = contact.Email,
                        EndPointAssessorOrganisationId = contact.EndPointAssessorOrganisationId,
                        OrganisationId = contact.OrganisationId,
                        PhoneNumber = contact.PhoneNumber,
                        Status = contact.Status,
                        Username = $"unknown-{ctr}"
                    }
                );
            }

            return contacts;
        }

        private static bool? ProcessYesNoValuesIntoBoolean(string integratedDegreeStandardValue)
        {
            bool? integratedDegreeStandard = null;
            if (integratedDegreeStandardValue == "N")
                integratedDegreeStandard = false;
            if (integratedDegreeStandardValue == "Y")
                integratedDegreeStandard = true;
            return integratedDegreeStandard;
        }
        private static DateTime ProcessValueAsDateTime(string valueIn, string fieldName, string worksheetName, int rowNumber)
        {
            if (DateTime.TryParse(valueIn, out DateTime valueParsed))
                return valueParsed;

            throw new MissingMandatoryDataException(
                $"Worksheet [{worksheetName}] has no suitable value for '{fieldName}' in row {rowNumber}");

        }
        private static DateTime? ProcessNullableDateValue(string valueIn)
        {
            if (DateTime.TryParse(valueIn, out DateTime valueParsed))
                return valueParsed;

            return null;
        }
        private static int? ProcessNullableIntValue(string valueIn)
        {
            if (int.TryParse(valueIn, out int valueParsed) && valueParsed > 0)
                return valueParsed;

            return null;
        }
        private static int ProcessValueAsInt(string valueIn, string fieldName, string worksheetName, int rowNumber)
        {
            if (int.TryParse(valueIn, out int valueParsed) && valueParsed > 0)
                return valueParsed;

            throw new MissingMandatoryDataException($"Worksheet [{worksheetName}] has no suitable value for '{fieldName}' in row {rowNumber}");
        }
        private static string ProcessPostcodeForExcessSpaces(object postcodeIn)
        {
            var postcode = postcodeIn?.ToString().Trim();

            if (postcode == null)
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
