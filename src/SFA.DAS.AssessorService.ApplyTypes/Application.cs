using SFA.DAS.AssessorService.Domain.Entities;
using System;

namespace SFA.DAS.AssessorService.ApplyTypes
{
    public class Application : ApplyTypeBase
    {
        public Domain.Entities.Organisation ApplyingOrganisation { get; set; }
        public Guid ApplyingOrganisationId { get; set; }
        public DateTime WithdrawnAt { get; set; }
        public string WithdrawnBy { get; set; }
        public string ApplicationStatus { get; set; }
        public ApplicationData ApplicationData { get; set; }
    }

    public class ApplicationData
    {
        public string ReferenceNumber { get; set; }

        // Preamble 
        public string OrganisationReferenceId { get; set; }
        public string OrganisationName { get; set; }
        public string OrganisationType { get; set; }
        public string ApplicationType { get; set; }

        // These are preamble answers, but are currently unused and stored instead in ApplyData.Apply
        public int? StandardCode { get; set; }
        public string StandardReference { get; set; }
        public string StandardName { get; set; }

        // This is a preamble answer
        public string Eqap { get; set; }

        public string TradingName { get; set; }
        public bool UseTradingName { get; set; }
        public string ContactGivenName { get; set; }
        
        public int? PipelinesCount { get; set; }
        public DateTime EarliestDateOfWithdrawal { get; set; }
        
        public DateTime ConfirmedWithdrawalDate { get; set; }

        public CompaniesHouseSummary CompanySummary { get; set; }
        public CharityCommissionSummary CharitySummary { get; set; }
    }
}
