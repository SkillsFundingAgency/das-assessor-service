namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class CertificatesCountResponse
    {
        public CertificatesCountResponse(int count)
        {
            Count = count;
        }
        public int Count { get; set; }
    }
}
