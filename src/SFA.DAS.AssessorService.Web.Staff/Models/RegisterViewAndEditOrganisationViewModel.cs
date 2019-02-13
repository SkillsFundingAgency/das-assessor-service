﻿using SFA.DAS.AssessorService.Api.Types.Models;
using System.Collections.Generic;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System;

namespace SFA.DAS.AssessorService.Web.Staff.Models
{
    public class RegisterViewAndEditOrganisationViewModel
    {
        public string OrganisationId { get; set; }
        public string Name { get; set; }
        public long? Ukprn { get; set; }
        public int? OrganisationTypeId { get; set; }
        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public string WebsiteLink { get; set; }
        public string Address { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Postcode { get; set; }
        public string OrganisationType { get; set; }
        public string PrimaryContact { get; set; }
        public string PrimaryContactName { get; set; }
        public string Status { get; set; }
        public List<ContactResponse> Contacts { get; set; }
        public string ContactsCount { get; set; }
        public string StandardsCount { get; set; }
        public List<OrganisationStandardSummary> OrganisationStandards { get; set; }
        public List<OrganisationType> OrganisationTypes { get; set; }
        public string CompanyNumber { get; set; }
        public string CharityNumber { get; set; }

        public string ActionChoice { get; set; }

        public DateTime? FinancialDueDate { get; set; }
        public bool? FinancialExempt { get; set; }
    }
}
