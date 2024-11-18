using System;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Models
{
    public class CertificateModel : TestModel
    {
        public Guid? Id { get; set; }
        public string CertificateData { get; set; }
        public DateTime? ToBePrinted { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }
        public string CertificateReference { get; set; }
        public Guid OrganisationId { get; set; }
        public int? BatchNumber { get; set; }
        public string Status { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public long Uln { get; set; }
        public int StandardCode { get; set; }
        public int ProviderUkPrn { get; set; }
        public int CertificateReferenceId { get; set; }
        public string LearnRefNumber { get; set; }
        public DateTime CreateDay { get; set; }
        public bool? IsPrivatelyFunded { get; set; }
        public string PrivatelyFundedStatus { get; set; }
        public string StandardUId { get; set; }
    }
}
