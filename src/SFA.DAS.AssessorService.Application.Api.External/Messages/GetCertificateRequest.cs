namespace SFA.DAS.AssessorService.Application.Api.External.Messages
{
    public class GetCertificateRequest
    {
        public long Uln { get; set; }
        public string FamilyName { get; set; }
        public int StandardCode { get; set; }

        public int UkPrn { get; set; }
        public string Email { get; set; }
    }
}
