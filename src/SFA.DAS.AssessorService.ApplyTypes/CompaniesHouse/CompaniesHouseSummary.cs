using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.ApplyTypes.CompaniesHouse
{
    public class CompaniesHouseSummary
    {
        private static IReadOnlyDictionary<string, string> CompanyTypeDescriptions = new Dictionary<string, string>
            {
                 ["private-unlimited"] = "Private unlimited company"
                ,["ltd"] = "Private limited company"
                ,["plc"] = "Public limited company"
                ,["old-public-company"] = "Old public company"
                ,["private-limited-guarant-nsc-limited-exemption"] = "Private Limited Company by guarantee without share capital, use of 'Limited' exemption"
                ,["limited-partnership"] = "Limited partnership"
                ,["private-limited-guarant-nsc"] = "Private limited by guarantee without share capital"
                ,["converted-or-closed"] = "Converted / closed"
                ,["private-unlimited-nsc"] = "Private unlimited company without share capital"
                ,["private-limited-shares-section-30-exemption"] = "Private Limited Company, use of 'Limited' exemption"
                ,["protected-cell-company"] = "Protected cell company"
                ,["assurance-company"] = "Assurance company"
                ,["oversea-company"] = "Overseas company"
                ,["eeig"] = "European economic interest grouping (EEIG)"
                ,["icvc-securities"] = "Investment company with variable capital"
                ,["icvc-warrant"] = "Investment company with variable capital"
                ,["icvc-umbrella"] = "Investment company with variable capital"
                ,["industrial-and-provident-society"] = "Industrial and provident society"
                ,["northern-ireland"] = "Northern Ireland company"
                ,["northern-ireland-other"] = "Credit union (Northern Ireland)"
                ,["llp"] = "Limited liability partnership"
                ,["royal-charter"] = "Royal charter company"
                ,["investment-company-with-variable-capital"] = "Investment company with variable capital"
                ,["unregistered-company"] = "Unregistered company"
                ,["other"] = "Other company type"
                ,["european-public-limited-liability-company-se"] = "European public limited liability company (SE)"
                ,["uk-establishment"] = "UK establishment company"
                ,["scottish-partnership"] = "Scottish qualifying partnership"
                ,["registered-society-non-jurisdictional"] = "Registered society"
                ,["charitable-incorporated-organisation"] = "Charitable incorporated organisation"
            };

        public string CompanyName { get; set; }
        public string CompanyNumber { get; set; }
        public string Status { get; set; }
        public string CompanyType { get; set; }
        public string CompanyTypeDescription
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CompanyType))
                {
                    return string.Empty;
                }
                return CompanyTypeDescriptions[CompanyType];
            }
        }

        public DateTime? IncorporationDate { get; set; }
        public List<DirectorInformation> Directors { get; set; }
        public List<PersonWithSignificantControlInformation> PersonsWithSignificantControl { get; set; }

        public bool ManualEntryRequired { get; set; }
    }

    public class DirectorInformation
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DateOfBirth { get; set; }
        public DateTime? AppointedDate { get; set; }
        public DateTime? ResignedDate { get; set; }

        public bool Active
        {
            get { return AppointedDate.HasValue && !ResignedDate.HasValue; }
        }
    }

    public class PersonWithSignificantControlInformation
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DateOfBirth { get; set; }
        public DateTime? NotifiedDate { get; set; }
        public DateTime? CeasedDate { get; set; }

        public bool Active
        {
            get { return !CeasedDate.HasValue; }
        }
    }
}
