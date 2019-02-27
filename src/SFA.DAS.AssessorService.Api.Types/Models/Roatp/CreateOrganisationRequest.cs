namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    using System;
    using MediatR;
    
    public class CreateOrganisationRequest : IRequest
    {
        public Organisation Organisation { get; set; }

        public string Username { get; set; }
    }

    public class Organisation
    {
        public Guid Id { get; set; }
        public ProviderType ProviderType { get; set; }
        public OrganisationType OrganisationType { get; set; }
        public long UKPRN { get; set; }
        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public OrganisationStatus OrganisationStatus { get; set; }
        public DateTime StatusDate { get; set; }
        public OrganisationData OrganisationData { get; set; }
    }

    public class OrganisationData
    {
        public string CompanyNumber { get; set; }
        public string CharityNumber { get; set; }
        public RemovedReason RemovedReason { get; set; }
        public bool ParentCompanyGuarantee { get; set; }
        public bool FinancialTrackRecord { get; set; }
        public bool NonLevyContract { get; set; }
    }

    public class ProviderType
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
    }

    public class RemovedReason
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public string Description { get; set; }

    }

    public class OrganisationStatus 
    {
        public int Id { get; set; }
        public string Status { get; set; }
    }

    public class OrganisationType 
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
    }
}
