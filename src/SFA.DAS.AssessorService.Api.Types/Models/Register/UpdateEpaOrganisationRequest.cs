﻿using System;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Api.Types.Models.Register
{
    public class UpdateEpaOrganisationRequest : IRequest<string>
    {
        public string Name { get; set; }
        public string OrganisationId { get; set; }
        public string Status { get; set; }
        public long? Ukprn { get; set; }
        public int? OrganisationTypeId { get; set; }
        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public string WebsiteLink { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Postcode { get; set; }
        public string ActionChoice { get; set; }
        public string CompanyNumber { get; set; }
        public string CharityNumber { get; set; }

        public DateTime? FinancialDueDate { get; set; }
        public bool? FinancialExempt { get; set; }
    
    }
}
