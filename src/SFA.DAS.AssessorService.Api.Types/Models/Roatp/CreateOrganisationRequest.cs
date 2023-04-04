using System;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    public class CreateOrganisationRequest : IRequest<bool>
    {
        public int ProviderTypeId { get; set; }
        public int OrganisationTypeId { get; set; }
        public string Ukprn { get; set; }
        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public DateTime StatusDate { get; set; }
        public string CharityNumber { get; set; }
        public string CompanyNumber { get; set; }
        public bool ParentCompanyGuarantee { get; set; }
        public bool FinancialTrackRecord { get; set; }
        public bool NonLevyContract { get; set; }

        public string Username { get; set; }
    }
}
